using Qahr.Mobile.Database;
using ProtoBuf;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Qahr.Mobile.Game.MsgServer
{
    [ProtoContract]
    public class MsgRankProto
    {
        [ProtoMember(1)]
        public ulong Mode;

        [ProtoMember(2)]
        public ulong ranktyp;

        [ProtoMember(3)]
        public ulong RegisteredCount;

        [ProtoMember(4)]
        public ulong Page;

        [ProtoMember(5)]
        public ulong Finish;

        [ProtoMember(6)]
        public ulong Count;

        [ProtoMember(7)]
        public ulong UID;

        [ProtoMember(8)]
        public List<MsgRankListPlayers> PlayerList = new List<MsgRankListPlayers>();
    }
    [ProtoContract]
    public class MsgRankListPlayers
    {
        [ProtoMember(1, IsRequired = true)]
        public ulong Rank;

        [ProtoMember(2, IsRequired = true)]
        public ulong Amount;

        [ProtoMember(3, IsRequired = true)]
        public ulong UID1;

        [ProtoMember(4, IsRequired = true)]
        public ulong UID2;

        [ProtoMember(5, IsRequired = true)]
        public string name;

        [ProtoMember(6, IsRequired = true)]
        public string name2;

        [ProtoMember(7, IsRequired = true)]
        public ulong Level;

        [ProtoMember(8, IsRequired = true)]
        public ulong Class;

        [ProtoMember(9, IsRequired = true)]
        public ulong Mesh;

        [ProtoMember(10, IsRequired = true)]
        public ulong Member10;

        [ProtoMember(11, IsRequired = true)]
        public ulong Member11;

        [ProtoMember(12, IsRequired = true)]
        public ulong AvatarFrame;
    }

    public static unsafe partial class MsgBuilder
    {
        public static unsafe ServerSockets.Packet CreateRank(this ServerSockets.Packet stream, MsgRankProto obj)
        {
            stream.InitWriter();
            stream.ProtoBufferSerialize(obj);
            stream.Finalize((ushort)GamePackets.MsgRank);
            return stream;
        }
        public static unsafe void GetGenericRanking(this ServerSockets.Packet stream, out MsgGenericRanking.Action Mode, out MsgGenericRanking.RankType ranktyp
            , out ushort RegisteredCount, out ushort Page, out int Count)
        {
            var Info = new MsgRankProto();
            Info = stream.ProtoBufferDeserialize(Info);
            Mode = (MsgGenericRanking.Action)Info.Mode;
            ranktyp = (MsgGenericRanking.RankType)Info.ranktyp;
            RegisteredCount = (ushort)Info.RegisteredCount;
            Page = (ushort)Info.Page;
            Count = (int)Info.Count;
        }
    }

    public unsafe struct MsgGenericRanking
    {
        public enum Action : uint
        {
            Ranking = 1,
            QueryCount = 2,
            UpdateScreen = 4,
            InformationRequest = 5,
            PrestigeRanks = 6,
            TotalPoints = 8,
            WeeklyRankings = 9,
            EonspiritAdvent = 10,
            TotalRanking = 11,
            RankFlower = 12,
        }
        public enum RankType : uint
        {
            None = 0,
            RoseFairy = 30000002,
            LilyFairy = 30000102,
            OrchidFairy = 30000202,
            TulipFairy = 30000302,
            Kiss = 30000402,
            Love = 30000502,
            Tins = 30000602,
            Jade = 30000702,
            YuanshenRank = 280000000,
            Chi = 60000000,
            DragonChi = 60000001,
            PhoenixChi = 60000002,
            TigerChi = 60000003,
            TurtleChi = 60000004,
            InnerPower = 70000000,
            PrestigeRank = 80000000,
            TopTrojans = 80000001,
            TopWarriors = 80000002,
            TopArchers = 80000003,
            TopNinjas = 80000004,
            TopMonks = 80000005,
            TopPirates = 80000006,
            TopDraonWarriors = 80000007,
            TopWaters = 80000008,
            TopFires = 80000009,
            TopWindWalker = 80000010,
            TopThunderstriker = 80000011,
            TopDuneWanderer = 80000012,
            WeeklyArcher = 10000300,
            WeeklyTao = 10000200,
            WeeklyPirate = 10000500,
            LeeLongWeekly = 10000600,
            ViodragonClubWeekly = 10000800,
            Rune = 110000000,
            HundredWeaponBegining = 120000000,
            HundredWeaponEnding = 120000010,
            Overall = 130000000,
            Fire = 130000001,
            Water = 130000002,
            Earth = 130000003,
            Wind = 130000004,
            Lightning = 130000005,
            Arena_Trojan = 140000001,
            Arena_Warrior = 140000002,
            Arena_Archer = 140000003,
            Arena_Ninja = 140000004,
            Arena_Monk = 140000005,
            Arena_Pirate = 140000006,
            Arena_DragonWarrior = 140000007,
            Arena_Water = 140000008,
            Arena_Fire = 140000009,
            Arena_Windwalker = 140000010,
            Arena_Thunderstriker = 140000011,
            Overall_Warrior = 150000000,
            HeroGatheringGathering = 180000000,
            HeroGatheringTrojan = 180000001,
            HeroGatheringWarrior = 180000002,
            HeroGatheringArcher = 180000004,
            HeroGatheringNinja = 180000005,
            HeroGatheringMonk = 180000006,
            HeroGatheringPirate = 180000007,
            HeroGatheringDragonWarrior = 180000008,
            HeroGatheringWater = 180000013,
            HeroGatheringFire = 180000014,
            HeroGatheringWindwalker = 180000016,
            HeroGatheringThunderstriker = 180000009,
            Dragonhowl = 150000001,
            Bloodlust = 150000002,
            Redcurse = 150000003,
            MonkTotalRanking = 300000000,
            HeavnlyTiger = 300000001,
            MightyDragon = 300000002,
            CosmicRoc = 300000003,
            MonkWeeklyRanking = 10001100,
            ArcherBegining = 190000000,
            ArcherEnding = 190000003,
            TaoBegining = 210000000,
            TaoEnding = 210000005,
            OverallRankings = 220000000,
            LeeLongTotalScore = 260000000,
            Dragon = 260000001,
            Kunpeng = 260000002,
            Suanni = 260000003,
            ViodragonClub = 270000001,
            LoveForEver = 270000002,
            HeartLock = 270000003,
            WarlordSpear = 270000004,
            JiangHuFameRanking = 230000003,
            JiangHuFameRankingTotal = 230000004,
            JiangHuFameRankingPrevious = 250000001,
            GirlsTotalFlower = 30000700,
            BoysTotalFlower = 30000800,
            #region Dune Archive Ranks
            DuneTotal = 320000000,//1
            Conception = 320000001,//1
            Governor = 320000002,//1
            Belt = 320000003,//1
            DuneWeekly = 10001200,//9
            #endregion
        }

        public static object SynRoot = new object();
        [PacketAttribute(GamePackets.MsgRank)]
        private static void Process(Client.GameClient user, ServerSockets.Packet stream)
        {
            const int max = 10;

            try
            {

                MsgGenericRanking.Action Mode;
                MsgGenericRanking.RankType ranktyp;
                ushort RegisteredCount;
                ushort Page;
                int Count;
                stream.GetGenericRanking(out Mode, out ranktyp, out RegisteredCount, out Page, out Count);

                switch (Mode)
                {
                    case Action.EonspiritAdvent:
                        {
                            var OldRank = ranktyp;
                            if (ranktyp == MsgGenericRanking.RankType.JiangHuFameRankingTotal)
                            {
                                var _RankType = Database.TableBetDiceRank.Type.JiangHuFameRankingTotal;
                                var rank = Database.TableBetDiceRank.Ranks[_RankType];
                                var array = rank.RankingList.Values.ToArray();
                                int offset = Page * max;
                                int count = System.Math.Min(max, array.Length);
                                var Info = new MsgRankProto()
                                {
                                    Mode = (ulong)Action.EonspiritAdvent,
                                    ranktyp = (ulong)OldRank,
                                    RegisteredCount = (ulong)array.Length,
                                    Page = Page,
                                    Finish = 1,
                                    Count = (ulong)count,
                                    UID = (ulong)count,
                                };
                                for (byte x = 0; x < count; x++)
                                {
                                    if (x + offset >= array.Length) break;
                                    var entity = array[x + offset];
                                    Info.PlayerList.Add(new MsgRankListPlayers()
                                    {
                                        Rank = (ulong)(offset + x + 1),
                                        Amount = (uint)entity.TotalPoints,
                                        UID1 = entity.UID,
                                        UID2 = entity.UID,
                                        name = entity.Name,
                                        name2 = entity.Name,
                                        Level = entity.Level,
                                        Class = entity.Class,
                                        Mesh = entity.Mesh,
                                        Member10 = 0,
                                        Member11 = 0,
                                        AvatarFrame = 0
                                    });
                                    if (x > 0 && x % 7 == 0 && count > 8)
                                    {
                                        user.Send(stream.CreateRank(Info));
                                        Info = new MsgRankProto()
                                        {
                                            Mode = (ulong)Action.EonspiritAdvent,
                                            ranktyp = (ulong)OldRank,
                                            RegisteredCount = (ulong)array.Length,
                                            Page = Page,
                                            Finish = 0,
                                            Count = (ulong)(count - (x + 1)),
                                            UID = (ulong)(count - (x + 1)),
                                        };
                                    }
                                }
                                user.Send(stream.CreateRank(Info));
                            }
                            if (ranktyp == MsgGenericRanking.RankType.JiangHuFameRankingPrevious)
                            {
                                var _RankType = Database.TableBetDiceRank.Type.JiangHuFameRankingPrevious;
                                var rank = Database.TableBetDiceRank.Ranks[_RankType];
                                var array = rank.RankingList.Values.ToArray();
                                int offset = Page * max;
                                int count = System.Math.Min(max, array.Length);
                                var Info = new MsgRankProto()
                                {
                                    Mode = (ulong)Action.EonspiritAdvent,
                                    ranktyp = (ulong)OldRank,
                                    RegisteredCount = (ulong)array.Length,
                                    Page = Page,
                                    Finish = 1,
                                    Count = (ulong)count,
                                    UID = (ulong)count,
                                };
                                for (byte x = 0; x < count; x++)
                                {
                                    if (x + offset >= array.Length) break;
                                    var entity = array[x + offset];
                                    Info.PlayerList.Add(new MsgRankListPlayers()
                                    {
                                        Rank = (ulong)(offset + x + 1),
                                        Amount = (uint)entity.TotalPoints,
                                        UID1 = entity.UID,
                                        UID2 = entity.UID,
                                        name = entity.Name,
                                        name2 = entity.Name,
                                        Level = entity.Level,
                                        Class = entity.Class,
                                        Mesh = entity.Mesh,
                                        Member10 = 0,
                                        Member11 = 0,
                                        AvatarFrame = 0
                                    });
                                    if (x > 0 && x % 7 == 0 && count > 8)
                                    {
                                        user.Send(stream.CreateRank(Info));
                                        Info = new MsgRankProto()
                                        {
                                            Mode = (ulong)Action.EonspiritAdvent,
                                            ranktyp = (ulong)OldRank,
                                            RegisteredCount = (ulong)array.Length,
                                            Page = Page,
                                            Finish = 0,
                                            Count = (ulong)(count - (x + 1)),
                                            UID = (ulong)(count - (x + 1)),
                                        };
                                    }
                                }
                                user.Send(stream.CreateRank(Info));
                            }
                            if (ranktyp == MsgGenericRanking.RankType.JiangHuFameRanking)
                            {
                                var _RankType = Database.TableBetDiceRank.Type.JiangHuFameRanking;
                                var rank = Database.TableBetDiceRank.Ranks[_RankType];
                                var array = rank.RankingList.Values.ToArray();
                                int offset = Page * max;
                                int count = System.Math.Min(max, array.Length);
                                var Info = new MsgRankProto()
                                {
                                    Mode = (ulong)Action.EonspiritAdvent,
                                    ranktyp = (ulong)OldRank,
                                    RegisteredCount = (ulong)array.Length,
                                    Page = Page,
                                    Finish = 1,
                                    Count = (ulong)count,
                                    UID = (ulong)count,
                                };
                                for (byte x = 0; x < count; x++)
                                {
                                    if (x + offset >= array.Length) break;
                                    var entity = array[x + offset];
                                    Info.PlayerList.Add(new MsgRankListPlayers()
                                    {
                                        Rank = (ulong)(offset + x + 1),
                                        Amount = (uint)entity.TotalPoints,
                                        UID1 = entity.UID,
                                        UID2 = entity.UID,
                                        name = entity.Name,
                                        name2 = entity.Name,
                                        Level = entity.Level,
                                        Class = entity.Class,
                                        Mesh = entity.Mesh,
                                        Member10 = 0,
                                        Member11 = 0,
                                        AvatarFrame = 0
                                    });
                                    if (x > 0 && x % 7 == 0 && count > 8)
                                    {
                                        user.Send(stream.CreateRank(Info));
                                        Info = new MsgRankProto()
                                        {
                                            Mode = (ulong)Action.EonspiritAdvent,
                                            ranktyp = (ulong)OldRank,
                                            RegisteredCount = (ulong)array.Length,
                                            Page = Page,
                                            Finish = 0,
                                            Count = (ulong)(count - (x + 1)),
                                            UID = (ulong)(count - (x + 1)),
                                        };
                                    }
                                }
                                user.Send(stream.CreateRank(Info));
                            }
                            break;
                        }

                    case Action.TotalRanking:
                        {
                            var OldRank = ranktyp;

                            //#region JiangHuFameRankingTotal
                            //if (ranktyp == MsgGenericRanking.RankType.JiangHuFameRankingTotal)// 99
                            //{

                            //    uint myRank = TableBetDiceRank.GetMyRank((TableBetDiceRank.Type)((int)ranktyp % 100), user.Player.UID);
                            //    var info = TableBetDiceRank.GetInfo((TableBetDiceRank.Type)((int)ranktyp % 100), user.Player.UID);
                            //    var proto = new MsgRankProto()
                            //    {
                            //        Mode = (ulong)Action.TotalRanking,
                            //        ranktyp = (ulong)ranktyp,
                            //        Page = 0,
                            //        Count = 1,
                            //        Finish = 1,
                            //        RegisteredCount = 1,
                            //    };
                            //    proto.PlayerList.Add(new MsgRankListPlayers()
                            //    {
                            //        Rank = info != null ? myRank : 0,
                            //        Amount = info != null ? info.TotalPoints : 0,
                            //        UID1 = info != null ? info.UID : 0,
                            //        UID2 = info != null ? info.UID : 0,
                            //        name = info != null ? info.Name : "",
                            //        name2 = info != null ? info.Name : "",
                            //        Level = info != null ? (byte)info.Level : (byte)0,
                            //        Class = info != null ? info.Class : 0,
                            //        Mesh = info != null ? info.Mesh : 0,
                            //        Member10 = 0,
                            //        Member11 = 0,
                            //        AvatarFrame = 0,
                            //    });
                            //    user.Send(stream.CreateRank(proto));
                            //    break;
                            //}
                            //#endregion
                            //#region JiangHuFameRanking
                            //if (ranktyp == MsgGenericRanking.RankType.JiangHuFameRanking)// 96
                            //{

                            //    uint myRank = TableBetDiceRank.GetMyRank((TableBetDiceRank.Type)((int)ranktyp % 100), user.Player.UID);
                            //    var info = TableBetDiceRank.GetInfo((TableBetDiceRank.Type)((int)ranktyp % 100), user.Player.UID);
                            //    var proto = new MsgRankProto()
                            //    {
                            //        Mode = (ulong)Action.TotalRanking,
                            //        ranktyp = (ulong)ranktyp,
                            //        Page = 0,
                            //        Count = 1,
                            //        Finish = 1,
                            //        RegisteredCount = 1,
                            //    };
                            //    proto.PlayerList.Add(new MsgRankListPlayers()
                            //    {
                            //        Rank = info != null ? myRank : 0,
                            //        Amount = info != null ? info.TotalPoints : 0,
                            //        UID1 = info != null ? info.UID : 0,
                            //        UID2 = info != null ? info.UID : 0,
                            //        name = info != null ? info.Name : "",
                            //        name2 = info != null ? info.Name : "",
                            //        Level = info != null ? (byte)info.Level : (byte)0,
                            //        Class = info != null ? info.Class : 0,
                            //        Mesh = info != null ? info.Mesh : 0,
                            //        Member10 = 0,
                            //        Member11 = 0,
                            //        AvatarFrame = 0,
                            //    });
                            //    user.Send(stream.CreateRank(proto));
                            //    break;
                            //}
                            //#endregion
                            #region JiangHuFameRankingPrevious
                            if (ranktyp == MsgGenericRanking.RankType.JiangHuFameRankingPrevious)// 97
                            {

                                uint myRank = TableBetDiceRank.GetMyRank((TableBetDiceRank.Type)((int)ranktyp % 100), user.Player.UID);
                                var info = TableBetDiceRank.GetInfo((TableBetDiceRank.Type)((int)ranktyp % 100), user.Player.UID);
                                var proto = new MsgRankProto()
                                {
                                    Mode = (ulong)Action.TotalRanking,
                                    ranktyp = (ulong)ranktyp,
                                    Page = 0,
                                    Count = 1,
                                    Finish = 1,
                                    RegisteredCount = 1,
                                };
                                proto.PlayerList.Add(new MsgRankListPlayers()
                                {
                                    Rank = info != null ? myRank : 0,
                                    Amount = info != null ? info.TotalPoints : 0,
                                    UID1 = info != null ? info.UID : 0,
                                    UID2 = info != null ? info.UID : 0,
                                    name = info != null ? info.Name : "",
                                    name2 = info != null ? info.Name : "",
                                    Level = info != null ? (byte)info.Level : (byte)0,
                                    Class = info != null ? info.Class : 0,
                                    Mesh = info != null ? info.Mesh : 0,
                                    Member10 = 0,
                                    Member11 = 0,
                                    AvatarFrame = 0,
                                });
                                user.Send(stream.CreateRank(proto));
                                break;
                            }
                            #endregion
                            break;
                        }
                    case Action.RankFlower:
                        {
                            if (Role.Core.IsGirl(user.Player.Body))
                            {
                                foreach (var item in Pool.GirlsFlowersRanking.RedRoses.Values.Where(o => o.Amount > 0).OrderByDescending(i => i.Amount).Take(1))
                                {
                                    if (Pool.GirlsFlowersRanking.RedRoses.Values.Count() == 0)
                                        break;
                                    if (item == null) continue;
                                    var proto = new MsgRankProto()
                                    {
                                        Mode = (ulong)Action.Ranking,
                                        ranktyp = (ulong)RankType.RoseFairy,
                                        Page = 0,
                                        Count = 1,
                                        Finish = 1,
                                        RegisteredCount = 1,
                                    };
                                    proto.PlayerList.Add(new MsgRankListPlayers()
                                    {
                                        Rank = item != null ? (ulong)item.Rank : (ulong)0,
                                        Amount = item != null ? item.Amount : 0,
                                        UID1 = item != null ? item.UID : 0,
                                        UID2 = item != null ? item.UID : 0,
                                        name = item != null ? item.Name : "",
                                        name2 = item != null ? item.Name : "",
                                        Level = 0,
                                        Class = 0,
                                        Mesh = item.Mesh,
                                        Member10 = 0,
                                        Member11 = 0,
                                        AvatarFrame = item.FrameID,
                                    });
                                    user.Send(stream.CreateRank(proto));
                                }
                                foreach (var item in Pool.GirlsFlowersRanking.Orchids.Values.Where(o => o.Amount > 0).OrderByDescending(i => i.Amount).Take(1))
                                {
                                    if (Pool.GirlsFlowersRanking.Orchids.Values.Count() == 0)
                                        break;
                                    if (item == null) continue;
                                    var proto = new MsgRankProto()
                                    {
                                        Mode = (ulong)Action.Ranking,
                                        ranktyp = (ulong)RankType.OrchidFairy,
                                        Page = 0,
                                        Count = 1,
                                        Finish = 1,
                                        RegisteredCount = 1,
                                    };
                                    proto.PlayerList.Add(new MsgRankListPlayers()
                                    {
                                        Rank = item != null ? (ulong)item.Rank : (ulong)0,
                                        Amount = item != null ? item.Amount : 0,
                                        UID1 = item != null ? item.UID : 0,
                                        UID2 = item != null ? item.UID : 0,
                                        name = item != null ? item.Name : "",
                                        name2 = item != null ? item.Name : "",
                                        Level = 0,
                                        Class = 0,
                                        Mesh = item.Mesh,
                                        Member10 = 0,
                                        Member11 = 0,
                                        AvatarFrame = item.FrameID,
                                    });
                                    user.Send(stream.CreateRank(proto));
                                }
                                foreach (var item in Pool.GirlsFlowersRanking.Lilies.Values.Where(o => o.Amount > 0).OrderByDescending(i => i.Amount).Take(1))
                                {
                                    if (Pool.GirlsFlowersRanking.Lilies.Values.Count() == 0)
                                        break;
                                    if (item == null) continue;
                                    var proto = new MsgRankProto()
                                    {
                                        Mode = (ulong)Action.Ranking,
                                        ranktyp = (ulong)RankType.LilyFairy,
                                        Page = 0,
                                        Count = 1,
                                        Finish = 1,
                                        RegisteredCount = 1,
                                    };
                                    proto.PlayerList.Add(new MsgRankListPlayers()
                                    {
                                        Rank = item != null ? (ulong)item.Rank : (ulong)0,
                                        Amount = item != null ? item.Amount : 0,
                                        UID1 = item != null ? item.UID : 0,
                                        UID2 = item != null ? item.UID : 0,
                                        name = item != null ? item.Name : "",
                                        name2 = item != null ? item.Name : "",
                                        Level = 0,
                                        Class = 0,
                                        Mesh = item.Mesh,
                                        Member10 = 0,
                                        Member11 = 0,
                                        AvatarFrame = item.FrameID,
                                    });
                                    user.Send(stream.CreateRank(proto));
                                }
                                foreach (var item in Pool.GirlsFlowersRanking.Tulips.Values.Where(o => o.Amount > 0).OrderByDescending(i => i.Amount).Take(1))
                                {
                                    if (Pool.GirlsFlowersRanking.Tulips.Values.Count() == 0)
                                        break;
                                    if (item == null) continue;
                                    var proto = new MsgRankProto()
                                    {
                                        Mode = (ulong)Action.Ranking,
                                        ranktyp = (ulong)RankType.TulipFairy,
                                        Page = 0,
                                        Count = 1,
                                        Finish = 1,
                                        RegisteredCount = 1,
                                    };
                                    proto.PlayerList.Add(new MsgRankListPlayers()
                                    {
                                        Rank = item != null ? (ulong)item.Rank : (ulong)0,
                                        Amount = item != null ? item.Amount : 0,
                                        UID1 = item != null ? item.UID : 0,
                                        UID2 = item != null ? item.UID : 0,
                                        name = item != null ? item.Name : "",
                                        name2 = item != null ? item.Name : "",
                                        Level = 0,
                                        Class = 0,
                                        Mesh = item.Mesh,
                                        Member10 = 0,
                                        Member11 = 0,
                                        AvatarFrame = item.FrameID,
                                    });
                                    user.Send(stream.CreateRank(proto));
                                }
                            }
                            else if (Role.Core.IsBoy(user.Player.Body))
                            {
                                foreach (var item in Pool.BoysFlowersRanking.RedRoses.Values.Where(o => o.Amount > 0).OrderByDescending(i => i.Amount).Take(1))
                                {
                                    if (Pool.BoysFlowersRanking.RedRoses.Values.Count() == 0)
                                        break;
                                    if (item == null) continue;
                                    var proto = new MsgRankProto()
                                    {
                                        Mode = (ulong)Action.Ranking,
                                        ranktyp = (ulong)RankType.Kiss,
                                        Page = 0,
                                        Count = 1,
                                        Finish = 1,
                                        RegisteredCount = 1,
                                    };
                                    proto.PlayerList.Add(new MsgRankListPlayers()
                                    {
                                        Rank = item != null ? (ulong)item.Rank : (ulong)0,
                                        Amount = item != null ? item.Amount : 0,
                                        UID1 = item != null ? item.UID : 0,
                                        UID2 = item != null ? item.UID : 0,
                                        name = item != null ? item.Name : "",
                                        name2 = item != null ? item.Name : "",
                                        Level = 0,
                                        Class = 0,
                                        Mesh = item.Mesh,
                                        Member10 = 0,
                                        Member11 = 0,
                                        AvatarFrame = item.FrameID,
                                    });
                                    user.Send(stream.CreateRank(proto));

                                }
                                foreach (var item in Pool.BoysFlowersRanking.Orchids.Values.Where(o => o.Amount > 0).OrderByDescending(i => i.Amount).Take(1))
                                {
                                    if (Pool.BoysFlowersRanking.Orchids.Values.Count() == 0)
                                        break;
                                    if (item == null) continue;

                                    var proto = new MsgRankProto()
                                    {
                                        Mode = (ulong)Action.Ranking,
                                        ranktyp = (ulong)RankType.Tins,
                                        Page = 0,
                                        Count = 1,
                                        Finish = 1,
                                        RegisteredCount = 1,
                                    };
                                    proto.PlayerList.Add(new MsgRankListPlayers()
                                    {
                                        Rank = item != null ? (ulong)item.Rank : (ulong)0,
                                        Amount = item != null ? item.Amount : 0,
                                        UID1 = item != null ? item.UID : 0,
                                        UID2 = item != null ? item.UID : 0,
                                        name = item != null ? item.Name : "",
                                        name2 = item != null ? item.Name : "",
                                        Level = 0,
                                        Class = 0,
                                        Mesh = item.Mesh,
                                        Member10 = 0,
                                        Member11 = 0,
                                        AvatarFrame = item.FrameID,
                                    });
                                    user.Send(stream.CreateRank(proto));
                                }
                                foreach (var item in Pool.BoysFlowersRanking.Lilies.Values.Where(o => o.Amount > 0).OrderByDescending(i => i.Amount).Take(1))
                                {
                                    if (Pool.BoysFlowersRanking.Lilies.Values.Count() == 0)
                                        break;
                                    if (item == null) continue;
                                    var proto = new MsgRankProto()
                                    {
                                        Mode = (ulong)Action.Ranking,
                                        ranktyp = (ulong)RankType.Love,
                                        Page = 0,
                                        Count = 1,
                                        Finish = 1,
                                        RegisteredCount = 1,
                                    };
                                    proto.PlayerList.Add(new MsgRankListPlayers()
                                    {
                                        Rank = item != null ? (ulong)item.Rank : (ulong)0,
                                        Amount = item != null ? item.Amount : 0,
                                        UID1 = item != null ? item.UID : 0,
                                        UID2 = item != null ? item.UID : 0,
                                        name = item != null ? item.Name : "",
                                        name2 = item != null ? item.Name : "",
                                        Level = 0,
                                        Class = 0,
                                        Mesh = item.Mesh,
                                        Member10 = 0,
                                        Member11 = 0,
                                        AvatarFrame = item.FrameID,
                                    });
                                    user.Send(stream.CreateRank(proto));
                                }
                                foreach (var item in Pool.BoysFlowersRanking.Tulips.Values.Where(o => o.Amount > 0).OrderByDescending(i => i.Amount).Take(1))
                                {
                                    if (Pool.BoysFlowersRanking.Tulips.Values.Count() == 0)
                                        break;
                                    if (item == null) continue;

                                    var proto = new MsgRankProto()
                                    {
                                        Mode = (ulong)Action.Ranking,
                                        ranktyp = (ulong)RankType.Jade,
                                        Page = 0,
                                        Count = 1,
                                        Finish = 1,
                                        RegisteredCount = 1,
                                    };
                                    proto.PlayerList.Add(new MsgRankListPlayers()
                                    {
                                        Rank = item != null ? (ulong)item.Rank : (ulong)0,
                                        Amount = item != null ? item.Amount : 0,
                                        UID1 = item != null ? item.UID : 0,
                                        UID2 = item != null ? item.UID : 0,
                                        name = item != null ? item.Name : "",
                                        name2 = item != null ? item.Name : "",
                                        Level = 0,
                                        Class = 0,
                                        Mesh = item.Mesh,
                                        Member10 = 0,
                                        Member11 = 0,
                                        AvatarFrame = item.FrameID,
                                    });
                                    user.Send(stream.CreateRank(proto));
                                }
                            }
                            break;
                        }
                    case Action.PrestigeRanks:
                        {
                            var array = Database.PrestigeRanking.Ranks.GetValues().Where(i => i.BestOfClass != null).OrderBy(p => p._Type).ToArray();
                            var Info = new MsgRankProto()
                            {
                                Mode = (ulong)Action.PrestigeRanks,
                                ranktyp = (ulong)0,
                                RegisteredCount = (ulong)0,
                                Page = 0,
                                Finish = 1,
                                Count = (ulong)array.Length,
                                UID = (ulong)array.Length,
                            };
                            for (int i = 0; i < array.Length; i++)
                            {
                                var BestOf = array[i].BestOfClass;
                                Info.PlayerList.Add(new MsgRankListPlayers()
                                {
                                    Rank = (ulong)(i + 1),
                                    Amount = (uint)BestOf.TotalPoints,
                                    UID1 = (uint)((int)RankType.PrestigeRank + (int)array[i]._Type),
                                    UID2 = BestOf.UID,
                                    name = BestOf.Name,
                                    name2 = BestOf.Name,
                                    Level = BestOf.Level,
                                    Class = BestOf.Class,
                                    Mesh = BestOf.Mesh,
                                    Member10 = 0,
                                    Member11 = 0,
                                    AvatarFrame = BestOf.FrameID
                                });
                                if (i > 0 && i % 7 == 0 && array.Length > 8)
                                {
                                    user.Send(stream.CreateRank(Info));
                                    Info = new MsgRankProto()
                                    {
                                        Mode = (ulong)Action.PrestigeRanks,
                                        ranktyp = (ulong)0,
                                        RegisteredCount = (ulong)0,
                                        Page = 0,
                                        Finish = 0,
                                        Count = (ulong)(array.Length - (i + 1)),
                                        UID = (ulong)(array.Length - (i + 1)),
                                    };
                                }
                            }
                            user.Send(stream.CreateRank(Info));
                            break;
                        }
                    case Action.QueryCount:
                        {
                            if ((RankType)ranktyp >= RankType.HeroGatheringGathering && (RankType)ranktyp <= RankType.HeroGatheringWindwalker)
                            {
                                uint Rank = HeroGatheringRank.GetMyRank((HeroGatheringRank.Type)((int)ranktyp % 100), user.Player.UID);
                                var info = HeroGatheringRank.GetInfo((HeroGatheringRank.Type)((int)ranktyp % 100), user.Player.UID);
                                var proto = new MsgRankProto()
                                {
                                    Mode = (ulong)Action.QueryCount,
                                    ranktyp = (ulong)ranktyp,
                                    Page = 0,
                                    Count = 1,
                                    Finish = 1,
                                    RegisteredCount = 1,
                                };
                                proto.PlayerList.Add(new MsgRankListPlayers()
                                {
                                    Rank = info != null ? Rank : 0,
                                    Amount = info != null ? info.TotalPoints : 0,
                                    UID1 = info != null ? info.UID : 0,
                                    UID2 = info != null ? info.UID : 0,
                                    name = info != null ? info.Name : "",
                                    name2 = info != null ? info.Name : "",
                                    Level = info != null ? (byte)info.Level : (byte)0,
                                    Class = info != null ? info.Class : 0,
                                    Mesh = info != null ? info.Mesh : 0,
                                    Member10 = 0,
                                    Member11 = 0,
                                    AvatarFrame = info != null ? info.AvatarFrame : 0,
                                });
                                user.Send(stream.CreateRank(proto));
                            }

                            #region Arena
                            if (ranktyp >= RankType.Arena_Trojan && ranktyp <= RankType.Arena_Thunderstriker)
                            {
                                uint[] Class = new uint[2] { 0, 0 };
                                if (ranktyp == RankType.Arena_Trojan) Class = new uint[2] { 1000, 1056 };
                                if (ranktyp == RankType.Arena_Warrior) Class = new uint[2] { 2000, 2056 };
                                if (ranktyp == RankType.Arena_Archer) Class = new uint[2] { 4000, 4056 };
                                if (ranktyp == RankType.Arena_Ninja) Class = new uint[2] { 5000, 5056 };
                                if (ranktyp == RankType.Arena_Monk) Class = new uint[2] { 6000, 6056 };
                                if (ranktyp == RankType.Arena_Pirate) Class = new uint[2] { 7000, 7056 };
                                if (ranktyp == RankType.Arena_DragonWarrior) Class = new uint[2] { 8000, 8056 };
                                if (ranktyp == RankType.Arena_Thunderstriker) Class = new uint[2] { 9000, 9056 };
                                if (ranktyp == RankType.Arena_Water) Class = new uint[2] { 13002, 13056 };
                                if (ranktyp == RankType.Arena_Fire) Class = new uint[2] { 14002, 14056 };
                                if (ranktyp == RankType.Arena_Windwalker) Class = new uint[2] { 16000, 16056 };
                                var info = MsgTournaments.MsgArena.Top1000Today.Where(p => p != null && p.UID == user.Player.UID && p.Info.ArenaPoints != 0 && p.Class >= Class[0] && p.Class <= Class[1]).FirstOrDefault();
                                var proto = new MsgRankProto()
                                {
                                    Mode = (ulong)Action.QueryCount,
                                    ranktyp = (ulong)ranktyp,
                                    Page = 0,
                                    Count = 1,
                                    Finish = 1,
                                    RegisteredCount = 1,
                                };
                                proto.PlayerList.Add(new MsgRankListPlayers()
                                {
                                    Rank = info != null ? info.Info.TodayRank : 0,
                                    Amount = info != null ? info.Info.ArenaPoints : 0,
                                    UID1 = info != null ? info.UID : 0,
                                    UID2 = info != null ? info.UID : 0,
                                    name = info != null ? info.Name : "",
                                    name2 = info != null ? info.Name : "",
                                    Level = info != null ? (byte)info.Level : (byte)0,
                                    Class = info != null ? info.Class : 0,
                                    Mesh = info != null ? info.Mesh : 0,
                                    Member10 = 0,
                                    Member11 = 0,
                                    AvatarFrame = 0,
                                });
                                user.Send(stream.CreateRank(proto));
                                break;
                            }

                            #endregion

                            #region PrestigeRank
                            if (ranktyp == RankType.PrestigeRank)
                            {
                                uint Rank = Database.PrestigeRanking.GetMyRank(Database.PrestigeRanking.Type.World, user.Player.UID);
                                if (Rank > Database.PrestigeRanking.Ranks[Database.PrestigeRanking.Type.World].MaxItems)
                                {
                                    Rank = 0;
                                }
                                var proto = new MsgRankProto()
                                {
                                    Mode = (ulong)Action.QueryCount,
                                    ranktyp = (ulong)RankType.PrestigeRank,
                                    Page = 0,
                                    Count = 1,
                                    Finish = 1,
                                    RegisteredCount = 1,
                                };
                                proto.PlayerList.Add(new MsgRankListPlayers()
                                {
                                    Rank = Rank,
                                    Amount = user.PrestrigeEntry.TotalPoints,
                                    UID1 = user.Player.UID,
                                    UID2 = user.Player.UID,
                                    name = user.Player.Name,
                                    name2 = user.Player.Name,
                                    Level = user.Player.Level,
                                    Class = user.Player.Class,
                                    Mesh = user.Player.Mesh,
                                    Member10 = 0,
                                    Member11 = 0,
                                    AvatarFrame = user.Player.FrameID,
                                });
                                user.Send(stream.CreateRank(proto));
                                break;
                            }
                            #endregion

                            #region ArchiveTrojan
                            if (ranktyp >= RankType.HundredWeaponBegining && ranktyp <= RankType.HundredWeaponEnding)
                            {
                                uint Rank = Database.HWRank.GetMyRank((Database.HWRank.Type)((int)ranktyp % 100), user.Player.UID);
                                var info = Database.HWRank.GetInfo((Database.HWRank.Type)((int)ranktyp % 100), user.Player.UID);
                                var proto = new MsgRankProto()
                                {
                                    Mode = (ulong)Action.QueryCount,
                                    ranktyp = (ulong)ranktyp,
                                    Page = 0,
                                    Count = 1,
                                    Finish = 1,
                                    RegisteredCount = 1,
                                };
                                proto.PlayerList.Add(new MsgRankListPlayers()
                                {
                                    Rank = info != null ? Rank : 0,
                                    Amount = info != null ? info.TotalPoints : 0,
                                    UID1 = info != null ? info.UID : 0,
                                    UID2 = info != null ? info.UID : 0,
                                    name = info != null ? info.Name : "",
                                    name2 = info != null ? info.Name : "",
                                    Level = info != null ? (byte)info.Level : (byte)0,
                                    Class = info != null ? info.Class : 0,
                                    Mesh = info != null ? info.Mesh : 0,
                                    Member10 = 0,
                                    Member11 = 0,
                                    AvatarFrame = 0,
                                });
                                user.Send(stream.CreateRank(proto));
                                break;
                            }
                            #endregion

                            #region Ninja
                            if (ranktyp >= RankType.Overall && ranktyp <= RankType.Lightning)
                            {
                                uint Rank = Database.NinjaRank.GetMyRank((Database.NinjaRank.Type)((int)ranktyp % 100), user.Player.UID);
                                var info = Database.NinjaRank.GetInfo((Database.NinjaRank.Type)((int)ranktyp % 100), user.Player.UID);
                                var proto = new MsgRankProto()
                                {
                                    Mode = (ulong)Action.QueryCount,
                                    ranktyp = (ulong)ranktyp,
                                    Page = 0,
                                    Count = 1,
                                    Finish = 1,
                                    RegisteredCount = 1,
                                };
                                proto.PlayerList.Add(new MsgRankListPlayers()
                                {
                                    Rank = info != null ? Rank : 0,
                                    Amount = info != null ? info.TotalPoints : 0,
                                    UID1 = info != null ? info.UID : 0,
                                    UID2 = info != null ? info.UID : 0,
                                    name = info != null ? info.Name : "",
                                    name2 = info != null ? info.Name : "",
                                    Level = info != null ? (byte)info.Level : (byte)0,
                                    Class = info != null ? info.Class : 0,
                                    Mesh = info != null ? info.Mesh : 0,
                                    Member10 = 0,
                                    Member11 = 0,
                                    AvatarFrame = 0,
                                });
                                user.Send(stream.CreateRank(proto));
                                break;
                            }
                            #endregion Ninja

                            #region ArchiveWarrior
                            if (ranktyp >= RankType.Overall_Warrior && ranktyp <= MsgGenericRanking.RankType.Redcurse)
                            {
                                uint myRank = WarriorRank.GetMyRank((WarriorRank.Type)((int)ranktyp % 100), user.Player.UID);
                                var info = WarriorRank.GetInfo((WarriorRank.Type)((int)ranktyp % 100), user.Player.UID);
                                var proto = new MsgRankProto()
                                {
                                    Mode = (ulong)Action.QueryCount,
                                    ranktyp = (ulong)ranktyp,
                                    Page = 0,
                                    Count = 1,
                                    Finish = 1,
                                    RegisteredCount = 1,
                                };
                                proto.PlayerList.Add(new MsgRankListPlayers()
                                {
                                    Rank = info != null ? myRank : 0,
                                    Amount = info != null ? info.TotalPoints : 0,
                                    UID1 = info != null ? info.UID : 0,
                                    UID2 = info != null ? info.UID : 0,
                                    name = info != null ? info.Name : "",
                                    name2 = info != null ? info.Name : "",
                                    Level = info != null ? (byte)info.Level : (byte)0,
                                    Class = info != null ? info.Class : 0,
                                    Mesh = info != null ? info.Mesh : 0,
                                    Member10 = 0,
                                    Member11 = 0,
                                    AvatarFrame = 0,
                                });
                                user.Send(stream.CreateRank(proto));
                                break;
                            }
                            #endregion

                            #region ArchiveArcher
                            if (ranktyp >= MsgGenericRanking.RankType.ArcherBegining && ranktyp <= MsgGenericRanking.RankType.ArcherEnding)
                            {
                                uint myRank = ArcherRank.GetMyRank((ArcherRank.Type)((int)ranktyp % 100), user.Player.UID);
                                var info = ArcherRank.GetInfo((ArcherRank.Type)((int)ranktyp % 100), user.Player.UID);
                                var proto = new MsgRankProto()
                                {
                                    Mode = (ulong)Action.QueryCount,
                                    ranktyp = (ulong)ranktyp,
                                    Page = 0,
                                    Count = 1,
                                    Finish = 1,
                                    RegisteredCount = 1,
                                };
                                proto.PlayerList.Add(new MsgRankListPlayers()
                                {
                                    Rank = info != null ? myRank : 0,
                                    Amount = info != null ? info.TotalPoints : 0,
                                    UID1 = info != null ? info.UID : 0,
                                    UID2 = info != null ? info.UID : 0,
                                    name = info != null ? info.Name : "",
                                    name2 = info != null ? info.Name : "",
                                    Level = info != null ? (byte)info.Level : (byte)0,
                                    Class = info != null ? info.Class : 0,
                                    Mesh = info != null ? info.Mesh : 0,
                                    Member10 = 0,
                                    Member11 = 0,
                                    AvatarFrame = 0,
                                });
                                user.Send(stream.CreateRank(proto));
                                break;
                            }
                            #endregion


                            #region ArchiveMonk
                            if (ranktyp >= MsgGenericRanking.RankType.MonkTotalRanking && ranktyp <= MsgGenericRanking.RankType.CosmicRoc)
                            {
                                uint myRank = MonkRanks.GetMyRank((MonkRanks.Type)((int)ranktyp % 100), user.Player.UID);
                                var info = MonkRanks.GetInfo((MonkRanks.Type)((int)ranktyp % 100), user.Player.UID);
                                var proto = new MsgRankProto()
                                {
                                    Mode = (ulong)Action.QueryCount,
                                    ranktyp = (ulong)ranktyp,
                                    Page = 0,
                                    Count = 1,
                                    Finish = 1,
                                    RegisteredCount = 1,
                                };
                                proto.PlayerList.Add(new MsgRankListPlayers()
                                {
                                    Rank = info != null ? myRank : 0,
                                    Amount = info != null ? info.TotalPoints : 0,
                                    UID1 = info != null ? info.UID : 0,
                                    UID2 = info != null ? info.UID : 0,
                                    name = info != null ? info.Name : "",
                                    name2 = info != null ? info.Name : "",
                                    Level = info != null ? (byte)info.Level : (byte)0,
                                    Class = info != null ? info.Class : 0,
                                    Mesh = info != null ? info.Mesh : 0,
                                    Member10 = 0,
                                    Member11 = 0,
                                    AvatarFrame = 0,
                                });
                                user.Send(stream.CreateRank(proto));
                                break;
                            }
                            #endregion

                            #region WaterRank
                            if (ranktyp >= MsgGenericRanking.RankType.TaoBegining && ranktyp <= MsgGenericRanking.RankType.TaoEnding && !AtributesStatus.IsFire(user.Player.Class))// any viewer; Fire class sees Fire list
                            {
                                uint myRank = WaterRank.GetMyRank((WaterRank.Type)((int)ranktyp % 100), user.Player.UID);
                                var info = WaterRank.GetInfo((WaterRank.Type)((int)ranktyp % 100), user.Player.UID);
                                var proto = new MsgRankProto()
                                {
                                    Mode = (ulong)Action.QueryCount,
                                    ranktyp = (ulong)ranktyp,
                                    Page = 0,
                                    Count = 1,
                                    Finish = 1,
                                    RegisteredCount = 1,
                                };
                                proto.PlayerList.Add(new MsgRankListPlayers()
                                {
                                    Rank = info != null ? myRank : 0,
                                    Amount = info != null ? info.TotalPoints : 0,
                                    UID1 = info != null ? info.UID : 0,
                                    UID2 = info != null ? info.UID : 0,
                                    name = info != null ? info.Name : "",
                                    name2 = info != null ? info.Name : "",
                                    Level = info != null ? (byte)info.Level : (byte)0,
                                    Class = info != null ? info.Class : 0,
                                    Mesh = info != null ? info.Mesh : 0,
                                    Member10 = 0,
                                    Member11 = 0,
                                    AvatarFrame = 0,
                                });
                                user.Send(stream.CreateRank(proto));
                                break;
                            }
                            #endregion

                            #region FireRank
                            if (ranktyp >= MsgGenericRanking.RankType.TaoBegining && ranktyp <= MsgGenericRanking.RankType.TaoEnding && AtributesStatus.IsFire(user.Player.Class))
                            {
                                uint myRank = FireRank.GetMyRank((FireRank.Type)((int)ranktyp % 100), user.Player.UID);
                                var info = FireRank.GetInfo((FireRank.Type)((int)ranktyp % 100), user.Player.UID);
                                var proto = new MsgRankProto()
                                {
                                    Mode = (ulong)Action.QueryCount,
                                    ranktyp = (ulong)ranktyp,
                                    Page = 0,
                                    Count = 1,
                                    Finish = 1,
                                    RegisteredCount = 1,
                                };
                                proto.PlayerList.Add(new MsgRankListPlayers()
                                {
                                    Rank = info != null ? myRank : 0,
                                    Amount = info != null ? info.TotalPoints : 0,
                                    UID1 = info != null ? info.UID : 0,
                                    UID2 = info != null ? info.UID : 0,
                                    name = info != null ? info.Name : "",
                                    name2 = info != null ? info.Name : "",
                                    Level = info != null ? (byte)info.Level : (byte)0,
                                    Class = info != null ? info.Class : 0,
                                    Mesh = info != null ? info.Mesh : 0,
                                    Member10 = 0,
                                    Member11 = 0,
                                    AvatarFrame = 0,
                                });
                                user.Send(stream.CreateRank(proto));
                                break;
                            }
                            #endregion

                            #region OverallRankings
                            if (ranktyp == MsgGenericRanking.RankType.OverallRankings)
                            {
                                uint myRank = PirateRank.GetMyRank((PirateRank.Type)((int)ranktyp % 100), user.Player.UID);
                                var info = PirateRank.GetInfo((PirateRank.Type)((int)ranktyp % 100), user.Player.UID);
                                var proto = new MsgRankProto()
                                {
                                    Mode = (ulong)Action.QueryCount,
                                    ranktyp = (ulong)ranktyp,
                                    Page = 0,
                                    Count = 1,
                                    Finish = 1,
                                    RegisteredCount = 1,
                                };
                                proto.PlayerList.Add(new MsgRankListPlayers()
                                {
                                    Rank = info != null ? myRank : 0,
                                    Amount = info != null ? info.TotalPoints : 0,
                                    UID1 = info != null ? info.UID : 0,
                                    UID2 = info != null ? info.UID : 0,
                                    name = info != null ? info.Name : "",
                                    name2 = info != null ? info.Name : "",
                                    Level = info != null ? (byte)info.Level : (byte)0,
                                    Class = info != null ? info.Class : 0,
                                    Mesh = info != null ? info.Mesh : 0,
                                    Member10 = 0,
                                    Member11 = 0,
                                    AvatarFrame = 0,
                                });
                                user.Send(stream.CreateRank(proto));
                                break;
                            }
                            #endregion

                            #region Dragon
                            if (ranktyp >= MsgGenericRanking.RankType.LeeLongTotalScore && ranktyp <= MsgGenericRanking.RankType.Suanni)
                            {
                                uint myRank = LeeLongRank.GetMyRank((LeeLongRank.Type)((int)ranktyp % 100), user.Player.UID);
                                var info = LeeLongRank.GetInfo((LeeLongRank.Type)((int)ranktyp % 100), user.Player.UID);
                                var proto = new MsgRankProto()
                                {
                                    Mode = (ulong)Action.QueryCount,
                                    ranktyp = (ulong)ranktyp,
                                    Page = 0,
                                    Count = 1,
                                    Finish = 1,
                                    RegisteredCount = 1,
                                };
                                proto.PlayerList.Add(new MsgRankListPlayers()
                                {
                                    Rank = info != null ? myRank : 0,
                                    Amount = info != null ? info.TotalPoints : 0,
                                    UID1 = info != null ? info.UID : 0,
                                    UID2 = info != null ? info.UID : 0,
                                    name = info != null ? info.Name : "",
                                    name2 = info != null ? info.Name : "",
                                    Level = info != null ? (byte)info.Level : (byte)0,
                                    Class = info != null ? info.Class : 0,
                                    Mesh = info != null ? info.Mesh : 0,
                                    Member10 = 0,
                                    Member11 = 0,
                                    AvatarFrame = 0,
                                });
                                user.Send(stream.CreateRank(proto));
                                break;
                            }
                            #endregion
                            #region Yuanshen

                            if (ranktyp == RankType.YuanshenRank)
                            {
                                uint Rank = YuanshenRank.GetMyRank(YuanshenRank.Type.Overall_EonSpirit, user.Player.UID);
                                var info = YuanshenRank.GetInfo(YuanshenRank.Type.Overall_EonSpirit, user.Player.UID);
                                var proto = new MsgRankProto()
                                {
                                    Mode = (ulong)Action.QueryCount,
                                    ranktyp = (ulong)ranktyp,
                                    Page = 0,
                                    Count = 1,
                                    Finish = 1,
                                    RegisteredCount = 1,
                                };
                                proto.PlayerList.Add(new MsgRankListPlayers()
                                {
                                    Rank = info != null ? Rank : 0,
                                    Amount = info != null ? info.TotalPoints : 0,
                                    UID1 = info != null ? info.UID : 0,
                                    UID2 = info != null ? info.UID : 0,
                                    name = info != null ? info.Name : "",
                                    name2 = info != null ? info.Name : "",
                                    Level = info != null ? (byte)info.Level : (byte)0,
                                    Class = info != null ? info.Class : 0,
                                    Mesh = info != null ? info.Mesh : 0,
                                    Member10 = 0,
                                    Member11 = 0,
                                    AvatarFrame = 0,
                                });
                                user.Send(stream.CreateRank(proto));
                                break;
                            }
                            #endregion

                            #region ViodragonClub
                            if (ranktyp == MsgGenericRanking.RankType.ViodragonClub)
                            {
                                uint myRank = AstredgeRank.GetMyRank(AstredgeRank.Type.ViodragonClub, user.Player.UID);
                                var info = AstredgeRank.GetInfo(AstredgeRank.Type.ViodragonClub, user.Player.UID);
                                var proto = new MsgRankProto()
                                {
                                    Mode = (ulong)Action.QueryCount,
                                    ranktyp = (ulong)ranktyp,
                                    Page = 0,
                                    Count = 1,
                                    Finish = 1,
                                    RegisteredCount = 1,
                                };
                                proto.PlayerList.Add(new MsgRankListPlayers()
                                {
                                    Rank = info != null ? myRank : 0,
                                    Amount = info != null ? info.TotalPoints : 0,
                                    UID1 = info != null ? info.UID : 0,
                                    UID2 = info != null ? info.UID : 0,
                                    name = info != null ? info.Name : "",
                                    name2 = info != null ? info.Name : "",
                                    Level = info != null ? (byte)info.Level : (byte)0,
                                    Class = info != null ? info.Class : 0,
                                    Mesh = info != null ? info.Mesh : 0,
                                    Member10 = 0,
                                    Member11 = 0,
                                    AvatarFrame = 0,
                                });
                                user.Send(stream.CreateRank(proto));
                                break;
                            }
                            #endregion
                            #region HeartLock
                            if (ranktyp == MsgGenericRanking.RankType.HeartLock)
                            {
                                uint myRank = AstredgeRank.GetMyRank(AstredgeRank.Type.HeartLock, user.Player.UID);
                                var info = AstredgeRank.GetInfo(AstredgeRank.Type.HeartLock, user.Player.UID);
                                var proto = new MsgRankProto()
                                {
                                    Mode = (ulong)Action.QueryCount,
                                    ranktyp = (ulong)ranktyp,
                                    Page = 0,
                                    Count = 1,
                                    Finish = 1,
                                    RegisteredCount = 1,
                                };
                                proto.PlayerList.Add(new MsgRankListPlayers()
                                {
                                    Rank = info != null ? myRank : 0,
                                    Amount = info != null ? info.TotalPoints : 0,
                                    UID1 = info != null ? info.UID : 0,
                                    UID2 = info != null ? info.UID : 0,
                                    name = info != null ? info.Name : "",
                                    name2 = info != null ? info.Name : "",
                                    Level = info != null ? (byte)info.Level : (byte)0,
                                    Class = info != null ? info.Class : 0,
                                    Mesh = info != null ? info.Mesh : 0,
                                    Member10 = 0,
                                    Member11 = 0,
                                    AvatarFrame = 0,
                                });
                                user.Send(stream.CreateRank(proto));
                                break;
                            }
                            #endregion
                            #region WarlordSpear
                            if (ranktyp == MsgGenericRanking.RankType.WarlordSpear)
                            {
                                uint myRank = AstredgeRank.GetMyRank(AstredgeRank.Type.WarlordSpear, user.Player.UID);
                                var info = AstredgeRank.GetInfo(AstredgeRank.Type.WarlordSpear, user.Player.UID);
                                var proto = new MsgRankProto()
                                {
                                    Mode = (ulong)Action.QueryCount,
                                    ranktyp = (ulong)ranktyp,
                                    Page = 0,
                                    Count = 1,
                                    Finish = 1,
                                    RegisteredCount = 1,
                                };
                                proto.PlayerList.Add(new MsgRankListPlayers()
                                {
                                    Rank = info != null ? myRank : 0,
                                    Amount = info != null ? info.TotalPoints : 0,
                                    UID1 = info != null ? info.UID : 0,
                                    UID2 = info != null ? info.UID : 0,
                                    name = info != null ? info.Name : "",
                                    name2 = info != null ? info.Name : "",
                                    Level = info != null ? (byte)info.Level : (byte)0,
                                    Class = info != null ? info.Class : 0,
                                    Mesh = info != null ? info.Mesh : 0,
                                    Member10 = 0,
                                    Member11 = 0,
                                    AvatarFrame = 0,
                                });
                                user.Send(stream.CreateRank(proto));
                                break;
                            }
                            #endregion
                            #region LoveForEver
                            if (ranktyp == MsgGenericRanking.RankType.LoveForEver)
                            {
                                uint myRank = AstredgeRank.GetMyRank(AstredgeRank.Type.LoveForEver, user.Player.UID);
                                var info = AstredgeRank.GetInfo(AstredgeRank.Type.LoveForEver, user.Player.UID);
                                var proto = new MsgRankProto()
                                {
                                    Mode = (ulong)Action.QueryCount,
                                    ranktyp = (ulong)ranktyp,
                                    Page = 0,
                                    Count = 1,
                                    Finish = 1,
                                    RegisteredCount = 1,
                                };
                                proto.PlayerList.Add(new MsgRankListPlayers()
                                {
                                    Rank = info != null ? myRank : 0,
                                    Amount = info != null ? info.TotalPoints : 0,
                                    UID1 = info != null ? info.UID : 0,
                                    UID2 = info != null ? info.UID : 0,
                                    name = info != null ? info.Name : "",
                                    name2 = info != null ? info.Name : "",
                                    Level = info != null ? (byte)info.Level : (byte)0,
                                    Class = info != null ? info.Class : 0,
                                    Mesh = info != null ? info.Mesh : 0,
                                    Member10 = 0,
                                    Member11 = 0,
                                    AvatarFrame = 0,
                                });
                                user.Send(stream.CreateRank(proto));
                                break;
                            }
                            #endregion

                            #region Flower
                            // only flower types use the flower fallback
                            if ((uint)ranktyp < 30000000 || (uint)ranktyp > 30000802)
                                break;
                            lock (SynRoot)
                            {
                                if (Role.Core.IsGirl(user.Player.Body))
                                    user.Player.Flowers.UpdateMyRank(user);
                                else if (Role.Core.IsBoy(user.Player.Body))
                                {
                                    foreach (var Flower in user.Player.Flowers)
                                    {
                                        if (Flower.Rank > 0 && Flower.Rank <= 100)
                                        {
                                            if (Flower.Type == MsgFlower.FlowersType.Tulips)
                                                ranktyp = RankType.Jade;
                                            if (Flower.Type == MsgFlower.FlowersType.Rouse)
                                                ranktyp = RankType.Kiss;
                                            if (Flower.Type == MsgFlower.FlowersType.Lilies)
                                                ranktyp = RankType.Love;
                                            if (Flower.Type == MsgFlower.FlowersType.Orchids)
                                                ranktyp = RankType.Tins;
                                            if (Flower.Type == MsgFlower.FlowersType.Peonies)
                                                ranktyp = RankType.BoysTotalFlower;
                                            var proto = new MsgRankProto()
                                            {
                                                Mode = (ulong)Action.QueryCount,
                                                ranktyp = (ulong)ranktyp,
                                                Page = 0,
                                                Count = 1,
                                                Finish = 1,
                                                RegisteredCount = 1,
                                            };
                                            proto.PlayerList.Add(new MsgRankListPlayers()
                                            {
                                                Rank = Flower != null ? (ulong)Flower.Rank : (ulong)0,
                                                Amount = Flower != null ? Flower.Amount : 0,
                                                UID1 = Flower != null ? Flower.UID : 0,
                                                UID2 = Flower != null ? Flower.UID : 0,
                                                name = Flower != null ? Flower.Name : "",
                                                name2 = Flower != null ? Flower.Name : "",
                                                Level = 0,
                                                Class = 0,
                                                Mesh = Flower.Mesh,
                                                Member10 = 0,
                                                Member11 = 0,
                                                AvatarFrame = 0,
                                            });
                                            user.Send(stream.CreateRank(proto));
                                        }
                                    }
                                    var protos = new MsgRankProto()
                                    {
                                        Mode = (ulong)Action.InformationRequest,
                                        ranktyp = (ulong)RankType.None,
                                        Page = 0,
                                        Count = 0,
                                        Finish = 1,
                                        RegisteredCount = 0,
                                    };
                                    user.Send(stream.CreateRank(protos));
                                }
                            }
                            #endregion
                            break;
                        }

                    case Action.Ranking:
                        {
                            var OldRank = ranktyp;
                            if ((RankType)ranktyp >= RankType.HeroGatheringGathering && (RankType)ranktyp <= RankType.HeroGatheringWindwalker)
                            {
                                var _RankType = (HeroGatheringRank.Type)((uint)ranktyp % 100);
                                var rank = HeroGatheringRank.Ranks[_RankType];
                                var array = rank.RankingList.Values.ToArray();
                                int offset = Page * max;
                                int count = Math.Min(max, array.Length);
                                var Info = new MsgRankProto()
                                {
                                    Mode = (ulong)Action.Ranking,
                                    ranktyp = (ulong)OldRank,
                                    RegisteredCount = (ulong)array.Length,
                                    Page = Page,
                                    Finish = 1,
                                    Count = (ulong)count,
                                    UID = (ulong)count,
                                };
                                for (byte x = 0; x < count; x++)
                                {
                                    if (x + offset >= array.Length)
                                        break;
                                    var entity = array[x + offset];
                                    Info.PlayerList.Add(new MsgRankListPlayers()
                                    {
                                        Rank = (ulong)(offset + x + 1),
                                        Amount = (uint)entity.TotalPoints,
                                        UID1 = entity.UID,
                                        UID2 = entity.UID,
                                        name = entity.Name,
                                        name2 = entity.Name,
                                        Level = entity.Level,
                                        Class = entity.Class,
                                        Mesh = entity.Mesh,
                                        Member10 = 0,
                                        Member11 = 0,
                                        AvatarFrame = entity.AvatarFrame
                                    });
                                    if (x > 0 && x % 7 == 0 && count > 8)
                                    {
                                        user.Send(stream.CreateRank(Info));
                                        Info = new MsgRankProto()
                                        {
                                            Mode = (ulong)Action.Ranking,
                                            ranktyp = (ulong)OldRank,
                                            RegisteredCount = (ulong)array.Length,
                                            Page = Page,
                                            Finish = 0,
                                            Count = (ulong)(count - (x + 1)),
                                            UID = (ulong)(count - (x + 1)),
                                        };
                                    }
                                }
                                user.Send(stream.CreateRank(Info));
                            }

                            #region PrestigeRank
                            if (ranktyp >= RankType.PrestigeRank && ranktyp <= RankType.TopDuneWanderer)
                            {
                                var _RankType = (Database.PrestigeRanking.Type)((uint)ranktyp % 100);
                                if (ranktyp == RankType.PrestigeRank)
                                    _RankType = Database.PrestigeRanking.Type.World;
                                var rank = Database.PrestigeRanking.Ranks[_RankType];
                                var array = rank.Items.Values.ToArray();
                                int offset = Page * max;
                                int count = Math.Min(max, array.Length);
                                var Info = new MsgRankProto()
                                {
                                    Mode = (ulong)Action.Ranking,
                                    ranktyp = (ulong)OldRank,
                                    RegisteredCount = (ulong)array.Length,
                                    Page = Page,
                                    Finish = 1,
                                    Count = (ulong)count,
                                    UID = (ulong)count,
                                };
                                for (byte x = 0; x < count; x++)
                                {
                                    if (x + offset >= array.Length)
                                        break;
                                    var entity = array[x + offset];
                                    Info.PlayerList.Add(new MsgRankListPlayers()
                                    {
                                        Rank = (ulong)(offset + x + 1),
                                        Amount = (uint)entity.TotalPoints,
                                        UID1 = entity.UID,
                                        UID2 = entity.UID,
                                        name = entity.Name,
                                        name2 = entity.Name,
                                        Level = entity.Level,
                                        Class = entity.Class,
                                        Mesh = entity.Mesh,
                                        Member10 = 0,
                                        Member11 = 0,
                                        AvatarFrame = 0
                                    });
                                    if (x > 0 && x % 7 == 0 && count > 8)
                                    {
                                        user.Send(stream.CreateRank(Info));
                                        Info = new MsgRankProto()
                                        {
                                            Mode = (ulong)Action.Ranking,
                                            ranktyp = (ulong)OldRank,
                                            RegisteredCount = (ulong)array.Length,
                                            Page = Page,
                                            Finish = 0,
                                            Count = (ulong)(count - (x + 1)),
                                            UID = (ulong)(count - (x + 1)),
                                        };
                                    }
                                }
                                user.Send(stream.CreateRank(Info));
                            }
                            #endregion

                            #region Arena
                            if (ranktyp >= RankType.Arena_Trojan && ranktyp <= RankType.Arena_Thunderstriker)
                            {
                                uint[] Class = new uint[2] { 0, 0 };
                                if (ranktyp == RankType.Arena_Trojan) Class = new uint[2] { 1000, 1056 };
                                if (ranktyp == RankType.Arena_Warrior) Class = new uint[2] { 2000, 2056 };
                                if (ranktyp == RankType.Arena_Archer) Class = new uint[2] { 4000, 4056 };
                                if (ranktyp == RankType.Arena_Ninja) Class = new uint[2] { 5000, 5056 };
                                if (ranktyp == RankType.Arena_Monk) Class = new uint[2] { 6000, 6056 };
                                if (ranktyp == RankType.Arena_Pirate) Class = new uint[2] { 7000, 7056 };
                                if (ranktyp == RankType.Arena_DragonWarrior) Class = new uint[2] { 8000, 8056 };
                                if (ranktyp == RankType.Arena_Thunderstriker) Class = new uint[2] { 9000, 9056 };
                                if (ranktyp == RankType.Arena_Water) Class = new uint[2] { 13002, 13056 };
                                if (ranktyp == RankType.Arena_Fire) Class = new uint[2] { 14002, 14056 };
                                if (ranktyp == RankType.Arena_Windwalker) Class = new uint[2] { 16000, 16056 };
                                var array = MsgTournaments.MsgArena.Top1000Today.Where(p => p != null && p.Info.ArenaPoints != 0 && p.Class >= Class[0] && p.Class <= Class[1]).OrderByDescending(p => p.Info.ArenaPoints).ToArray();
                                int offset = Page * max;
                                int count = System.Math.Min(max, array.Length);
                                var Info = new MsgRankProto()
                                {
                                    Mode = (ulong)Action.Ranking,
                                    ranktyp = (ulong)OldRank,
                                    RegisteredCount = (ulong)array.Length,
                                    Page = Page,
                                    Finish = 1,
                                    Count = (ulong)count,
                                    UID = (ulong)count,
                                };
                                for (byte x = 0; x < count; x++)
                                {
                                    if (x + offset >= array.Length) break;
                                    var entity = array[x + offset];
                                    Info.PlayerList.Add(new MsgRankListPlayers()
                                    {
                                        Rank = (ulong)(offset + x + 1),
                                        Amount = (uint)entity.Info.ArenaPoints,
                                        UID1 = entity.UID,
                                        UID2 = entity.UID,
                                        name = entity.Name,
                                        name2 = entity.Name,
                                        Level = entity.Level,
                                        Class = entity.Class,
                                        Mesh = entity.Mesh,
                                        Member10 = 0,
                                        Member11 = 0,
                                        AvatarFrame = 0
                                    });
                                    if (x > 0 && x % 7 == 0 && count > 8)
                                    {
                                        user.Send(stream.CreateRank(Info));
                                        Info = new MsgRankProto()
                                        {
                                            Mode = (ulong)Action.Ranking,
                                            ranktyp = (ulong)OldRank,
                                            RegisteredCount = (ulong)array.Length,
                                            Page = Page,
                                            Finish = 0,
                                            Count = (ulong)(count - (x + 1)),
                                            UID = (ulong)(count - (x + 1)),
                                        };
                                    }
                                }
                                user.Send(stream.CreateRank(Info));
                                break;
                            }

                            #endregion

                            #region ArchiveTrojan
                            if (ranktyp >= RankType.HundredWeaponBegining && ranktyp <= RankType.HundredWeaponEnding)
                            {
                                var _RankType = (Database.HWRank.Type)((uint)ranktyp % 100);
                                var rank = Database.HWRank.Ranks[_RankType];
                                var array = rank.RankingList.Values.ToArray();
                                int offset = Page * max;
                                int count = System.Math.Min(max, array.Length);
                                var Info = new MsgRankProto()
                                {
                                    Mode = (ulong)Action.Ranking,
                                    ranktyp = (ulong)OldRank,
                                    RegisteredCount = (ulong)array.Length,
                                    Page = Page,
                                    Finish = 1,
                                    Count = (ulong)count,
                                    UID = (ulong)count,
                                };
                                for (byte x = 0; x < count; x++)
                                {
                                    if (x + offset >= array.Length) break;
                                    var entity = array[x + offset];
                                    Info.PlayerList.Add(new MsgRankListPlayers()
                                    {
                                        Rank = (ulong)(offset + x + 1),
                                        Amount = (uint)entity.TotalPoints,
                                        UID1 = entity.UID,
                                        UID2 = entity.UID,
                                        name = entity.Name,
                                        name2 = entity.Name,
                                        Level = entity.Level,
                                        Class = entity.Class,
                                        Mesh = entity.Mesh,
                                        Member10 = 0,
                                        Member11 = 0,
                                        AvatarFrame = 0
                                    });
                                    if (x > 0 && x % 7 == 0 && count > 8)
                                    {
                                        user.Send(stream.CreateRank(Info));
                                        Info = new MsgRankProto()
                                        {
                                            Mode = (ulong)Action.Ranking,
                                            ranktyp = (ulong)OldRank,
                                            RegisteredCount = (ulong)array.Length,
                                            Page = Page,
                                            Finish = 0,
                                            Count = (ulong)(count - (x + 1)),
                                            UID = (ulong)(count - (x + 1)),
                                        };
                                    }
                                }
                                user.Send(stream.CreateRank(Info));
                                break;
                            }
                            #endregion

                            #region ArchiveNinja
                            if (ranktyp >= RankType.Overall && ranktyp <= RankType.Lightning)
                            {
                                var _RankType = (NinjaRank.Type)((uint)ranktyp % 100);
                                var rank = NinjaRank.Ranks[_RankType];
                                var array = rank.RankingList.Values.ToArray();
                                int offset = Page * max;
                                int count = System.Math.Min(max, array.Length);
                                var Info = new MsgRankProto()
                                {
                                    Mode = (ulong)Action.Ranking,
                                    ranktyp = (ulong)OldRank,
                                    RegisteredCount = (ulong)array.Length,
                                    Page = Page,
                                    Finish = 1,
                                    Count = (ulong)count,
                                    UID = (ulong)count,
                                };
                                for (byte x = 0; x < count; x++)
                                {
                                    if (x + offset >= array.Length) break;
                                    var entity = array[x + offset];
                                    Info.PlayerList.Add(new MsgRankListPlayers()
                                    {
                                        Rank = (ulong)(offset + x + 1),
                                        Amount = (uint)entity.TotalPoints,
                                        UID1 = entity.UID,
                                        UID2 = entity.UID,
                                        name = entity.Name,
                                        name2 = entity.Name,
                                        Level = entity.Level,
                                        Class = entity.Class,
                                        Mesh = entity.Mesh,
                                        Member10 = 0,
                                        Member11 = 0,
                                        AvatarFrame = 0
                                    });
                                    if (x > 0 && x % 7 == 0 && count > 8)
                                    {
                                        user.Send(stream.CreateRank(Info));
                                        Info = new MsgRankProto()
                                        {
                                            Mode = (ulong)Action.Ranking,
                                            ranktyp = (ulong)OldRank,
                                            RegisteredCount = (ulong)array.Length,
                                            Page = Page,
                                            Finish = 0,
                                            Count = (ulong)(count - (x + 1)),
                                            UID = (ulong)(count - (x + 1)),
                                        };
                                    }
                                }
                                user.Send(stream.CreateRank(Info));
                                break;
                            }


                            #endregion Ninja

                            #region ArchiveWarrior
                            else if (ranktyp >= MsgGenericRanking.RankType.Overall_Warrior && ranktyp <= MsgGenericRanking.RankType.Redcurse)
                            {
                                WarriorRank.Type key = (WarriorRank.Type)((uint)ranktyp % 100);
                                WarriorRank.Entry[] array = WarriorRank.Ranks[key].RankingList.Values.ToArray<WarriorRank.Entry>();
                                int num = (int)Page * 10;
                                int offset = Page * max;
                                int count = Math.Min(10, array.Length);
                                var Info = new MsgRankProto()
                                {
                                    Mode = (ulong)Action.Ranking,
                                    ranktyp = (ulong)OldRank,
                                    RegisteredCount = (ulong)array.Length,
                                    Page = Page,
                                    Finish = 1,
                                    Count = (ulong)count,
                                    UID = (ulong)count,
                                };
                                for (byte x = 0; x < count; x++)
                                {
                                    if (x + offset >= array.Length) break;
                                    var entity = array[x + offset];
                                    Info.PlayerList.Add(new MsgRankListPlayers()
                                    {
                                        Rank = (ulong)(offset + x + 1),
                                        Amount = (uint)entity.TotalPoints,
                                        UID1 = entity.UID,
                                        UID2 = entity.UID,
                                        name = entity.Name,
                                        name2 = entity.Name,
                                        Level = entity.Level,
                                        Class = entity.Class,
                                        Mesh = entity.Mesh,
                                        Member10 = 0,
                                        Member11 = 0,
                                        AvatarFrame = 0
                                    });
                                    if (x > 0 && x % 7 == 0 && count > 8)
                                    {
                                        user.Send(stream.CreateRank(Info));
                                        Info = new MsgRankProto()
                                        {
                                            Mode = (ulong)Action.Ranking,
                                            ranktyp = (ulong)OldRank,
                                            RegisteredCount = (ulong)array.Length,
                                            Page = Page,
                                            Finish = 0,
                                            Count = (ulong)(count - (x + 1)),
                                            UID = (ulong)(count - (x + 1)),
                                        };
                                    }
                                }
                                user.Send(stream.CreateRank(Info));
                            }
                            #endregion

                            #region ArchiveArcher
                            else if (ranktyp >= MsgGenericRanking.RankType.ArcherBegining && ranktyp <= MsgGenericRanking.RankType.ArcherEnding)
                            {
                                ArcherRank.Type key = (ArcherRank.Type)((uint)ranktyp % 100);
                                ArcherRank.Entry[] array = ArcherRank.Ranks[key].RankingList.Values.ToArray<ArcherRank.Entry>();
                                int num = (int)Page * 10;
                                int offset = Page * max;
                                int count = Math.Min(10, array.Length);
                                var Info = new MsgRankProto()
                                {
                                    Mode = (ulong)Action.Ranking,
                                    ranktyp = (ulong)OldRank,
                                    RegisteredCount = (ulong)array.Length,
                                    Page = Page,
                                    Finish = 1,
                                    Count = (ulong)count,
                                    UID = (ulong)count,
                                };
                                for (byte x = 0; x < count; x++)
                                {
                                    if (x + offset >= array.Length) break;
                                    var entity = array[x + offset];
                                    Info.PlayerList.Add(new MsgRankListPlayers()
                                    {
                                        Rank = (ulong)(offset + x + 1),
                                        Amount = (uint)entity.TotalPoints,
                                        UID1 = entity.UID,
                                        UID2 = entity.UID,
                                        name = entity.Name,
                                        name2 = entity.Name,
                                        Level = entity.Level,
                                        Class = entity.Class,
                                        Mesh = entity.Mesh,
                                        Member10 = 0,
                                        Member11 = 0,
                                        AvatarFrame = 0
                                    });
                                    if (x > 0 && x % 7 == 0 && count > 8)
                                    {
                                        user.Send(stream.CreateRank(Info));
                                        Info = new MsgRankProto()
                                        {
                                            Mode = (ulong)Action.Ranking,
                                            ranktyp = (ulong)OldRank,
                                            RegisteredCount = (ulong)array.Length,
                                            Page = Page,
                                            Finish = 0,
                                            Count = (ulong)(count - (x + 1)),
                                            UID = (ulong)(count - (x + 1)),
                                        };
                                    }
                                }
                                user.Send(stream.CreateRank(Info));
                            }
                            #endregion

                            #region DuneWandererRank
                            else if (ranktyp >= MsgGenericRanking.RankType.DuneTotal && ranktyp <= MsgGenericRanking.RankType.Belt)
                            {
                                DuneWandererRank.Type key = (DuneWandererRank.Type)((uint)ranktyp % 100);
                                DuneWandererRank.Entry[] array = DuneWandererRank.Ranks[key].RankingList.Values.ToArray<DuneWandererRank.Entry>();
                                int num = (int)Page * 10;
                                int offset = Page * max;
                                int count = Math.Min(10, array.Length);
                                var Info = new MsgRankProto()
                                {
                                    Mode = (ulong)Action.Ranking,
                                    ranktyp = (ulong)OldRank,
                                    RegisteredCount = (ulong)array.Length,
                                    Page = Page,
                                    Finish = 1,
                                    Count = (ulong)count,
                                    UID = (ulong)count,
                                };
                                for (byte x = 0; x < count; x++)
                                {
                                    if (x + offset >= array.Length)
                                    {
                                        break;
                                    }

                                    var entity = array[x + offset];
                                    Info.PlayerList.Add(new MsgRankListPlayers()
                                    {
                                        Rank = (ulong)(offset + x + 1),
                                        Amount = (uint)entity.TotalPoints,
                                        UID1 = entity.UID,
                                        UID2 = entity.UID,
                                        name = entity.Name,
                                        name2 = entity.Name,
                                        Level = entity.Level,
                                        Class = entity.Class,
                                        Mesh = entity.Mesh,
                                        Member10 = 0,
                                        Member11 = 0,
                                        AvatarFrame = 0
                                    });
                                    if (x > 0 && x % 7 == 0 && count > 8)
                                    {
                                        user.Send(stream.CreateRank(Info));
                                        Info = new MsgRankProto()
                                        {
                                            Mode = (ulong)Action.Ranking,
                                            ranktyp = (ulong)OldRank,
                                            RegisteredCount = (ulong)array.Length,
                                            Page = Page,
                                            Finish = 0,
                                            Count = (ulong)(count - (x + 1)),
                                            UID = (ulong)(count - (x + 1)),
                                        };
                                    }
                                }
                                user.Send(stream.CreateRank(Info));
                            }
                            #endregion

                            #region ArchiveMonk
                            else if (ranktyp >= MsgGenericRanking.RankType.MonkTotalRanking && ranktyp <= MsgGenericRanking.RankType.CosmicRoc)
                            {
                                MonkRanks.Type key = (MonkRanks.Type)((uint)ranktyp % 100);
                                MonkRanks.Entry[] array = MonkRanks.Ranks[key].RankingList.Values.ToArray<MonkRanks.Entry>();
                                int num = (int)Page * 10;
                                int Count4 = Math.Min(10, array.Length);
                                int offset = Page * max;
                                int count = Math.Min(10, array.Length);
                                var Info = new MsgRankProto()
                                {
                                    Mode = (ulong)Action.Ranking,
                                    ranktyp = (ulong)OldRank,
                                    RegisteredCount = (ulong)array.Length,
                                    Page = Page,
                                    Finish = 1,
                                    Count = (ulong)count,
                                    UID = (ulong)count,
                                };
                                for (byte x = 0; x < count; x++)
                                {
                                    if (x + offset >= array.Length) break;
                                    var entity = array[x + offset];
                                    Info.PlayerList.Add(new MsgRankListPlayers()
                                    {
                                        Rank = (ulong)(offset + x + 1),
                                        Amount = (uint)entity.TotalPoints,
                                        UID1 = entity.UID,
                                        UID2 = entity.UID,
                                        name = entity.Name,
                                        name2 = entity.Name,
                                        Level = entity.Level,
                                        Class = entity.Class,
                                        Mesh = entity.Mesh,
                                        Member10 = 0,
                                        Member11 = 0,
                                        AvatarFrame = 0
                                    });
                                    if (x > 0 && x % 7 == 0 && count > 8)
                                    {
                                        user.Send(stream.CreateRank(Info));
                                        Info = new MsgRankProto()
                                        {
                                            Mode = (ulong)Action.Ranking,
                                            ranktyp = (ulong)OldRank,
                                            RegisteredCount = (ulong)array.Length,
                                            Page = Page,
                                            Finish = 0,
                                            Count = (ulong)(count - (x + 1)),
                                            UID = (ulong)(count - (x + 1)),
                                        };
                                    }
                                }
                                user.Send(stream.CreateRank(Info));
                            }
                            #endregion
                            #region WaterRank
                            else if (ranktyp >= MsgGenericRanking.RankType.TaoBegining && ranktyp <= MsgGenericRanking.RankType.TaoEnding && !AtributesStatus.IsFire(user.Player.Class))// any viewer; Fire class sees Fire list
                            {
                                WaterRank.Type key = (WaterRank.Type)((uint)ranktyp % 100);
                                WaterRank.Entry[] array = WaterRank.Ranks[key].RankingList.Values.ToArray<WaterRank.Entry>();
                                int num = (int)Page * 10;
                                int Count4 = Math.Min(10, array.Length);
                                int offset = Page * max;
                                int count = Math.Min(10, array.Length);
                                var Info = new MsgRankProto()
                                {
                                    Mode = (ulong)Action.Ranking,
                                    ranktyp = (ulong)OldRank,
                                    RegisteredCount = (ulong)array.Length,
                                    Page = Page,
                                    Finish = 1,
                                    Count = (ulong)count,
                                    UID = (ulong)count,
                                };
                                for (byte x = 0; x < count; x++)
                                {
                                    if (x + offset >= array.Length) break;
                                    var entity = array[x + offset];
                                    Info.PlayerList.Add(new MsgRankListPlayers()
                                    {
                                        Rank = (ulong)(offset + x + 1),
                                        Amount = (uint)entity.TotalPoints,
                                        UID1 = entity.UID,
                                        UID2 = entity.UID,
                                        name = entity.Name,
                                        name2 = entity.Name,
                                        Level = entity.Level,
                                        Class = entity.Class,
                                        Mesh = entity.Mesh,
                                        Member10 = 0,
                                        Member11 = 0,
                                        AvatarFrame = 0
                                    });
                                    if (x > 0 && x % 7 == 0 && count > 8)
                                    {
                                        user.Send(stream.CreateRank(Info));
                                        Info = new MsgRankProto()
                                        {
                                            Mode = (ulong)Action.Ranking,
                                            ranktyp = (ulong)OldRank,
                                            RegisteredCount = (ulong)array.Length,
                                            Page = Page,
                                            Finish = 0,
                                            Count = (ulong)(count - (x + 1)),
                                            UID = (ulong)(count - (x + 1)),
                                        };
                                    }
                                }
                                user.Send(stream.CreateRank(Info));
                            }
                            #endregion

                            #region FireRank
                            else if (ranktyp >= MsgGenericRanking.RankType.TaoBegining && ranktyp <= MsgGenericRanking.RankType.TaoEnding && AtributesStatus.IsFire(user.Player.Class))
                            {
                                FireRank.Type key = (FireRank.Type)((uint)ranktyp % 100);
                                FireRank.Entry[] array = FireRank.Ranks[key].RankingList.Values.ToArray<FireRank.Entry>();
                                int num = (int)Page * 10;
                                int Count4 = Math.Min(10, array.Length);
                                int offset = Page * max;
                                int count = Math.Min(10, array.Length);
                                var Info = new MsgRankProto()
                                {
                                    Mode = (ulong)Action.Ranking,
                                    ranktyp = (ulong)OldRank,
                                    RegisteredCount = (ulong)array.Length,
                                    Page = Page,
                                    Finish = 1,
                                    Count = (ulong)count,
                                    UID = (ulong)count,
                                };
                                for (byte x = 0; x < count; x++)
                                {
                                    if (x + offset >= array.Length) break;
                                    var entity = array[x + offset];
                                    Info.PlayerList.Add(new MsgRankListPlayers()
                                    {
                                        Rank = (ulong)(offset + x + 1),
                                        Amount = (uint)entity.TotalPoints,
                                        UID1 = entity.UID,
                                        UID2 = entity.UID,
                                        name = entity.Name,
                                        name2 = entity.Name,
                                        Level = entity.Level,
                                        Class = entity.Class,
                                        Mesh = entity.Mesh,
                                        Member10 = 0,
                                        Member11 = 0,
                                        AvatarFrame = 0
                                    });
                                    if (x > 0 && x % 7 == 0 && count > 8)
                                    {
                                        user.Send(stream.CreateRank(Info));
                                        Info = new MsgRankProto()
                                        {
                                            Mode = (ulong)Action.Ranking,
                                            ranktyp = (ulong)OldRank,
                                            RegisteredCount = (ulong)array.Length,
                                            Page = Page,
                                            Finish = 0,
                                            Count = (ulong)(count - (x + 1)),
                                            UID = (ulong)(count - (x + 1)),
                                        };
                                    }
                                }
                                user.Send(stream.CreateRank(Info));
                            }
                            #endregion

                            #region OverallRankings
                            if (ranktyp == MsgGenericRanking.RankType.OverallRankings)
                            {
                                PirateRank.Type key = (PirateRank.Type)((uint)ranktyp % 100);
                                PirateRank.Entry[] array = PirateRank.Ranks[key].RankingList.Values.ToArray<PirateRank.Entry>();
                                int offset = Page * max;
                                int count = Math.Min(10, array.Length);
                                var Info = new MsgRankProto()
                                {
                                    Mode = (ulong)Action.Ranking,
                                    ranktyp = (ulong)OldRank,
                                    RegisteredCount = (ulong)array.Length,
                                    Page = Page,
                                    Finish = 1,
                                    Count = (ulong)count,
                                    UID = (ulong)count,
                                };
                                for (byte x = 0; x < count; x++)
                                {
                                    if (x + offset >= array.Length) break;
                                    var entity = array[x + offset];
                                    Info.PlayerList.Add(new MsgRankListPlayers()
                                    {
                                        Rank = (ulong)(offset + x + 1),
                                        Amount = (uint)entity.TotalPoints,
                                        UID1 = entity.UID,
                                        UID2 = entity.UID,
                                        name = entity.Name,
                                        name2 = entity.Name,
                                        Level = entity.Level,
                                        Class = entity.Class,
                                        Mesh = entity.Mesh,
                                        Member10 = 0,
                                        Member11 = 0,
                                        AvatarFrame = 0
                                    });
                                    if (x > 0 && x % 7 == 0 && count > 8)
                                    {
                                        user.Send(stream.CreateRank(Info));
                                        Info = new MsgRankProto()
                                        {
                                            Mode = (ulong)Action.Ranking,
                                            ranktyp = (ulong)OldRank,
                                            RegisteredCount = (ulong)array.Length,
                                            Page = Page,
                                            Finish = 0,
                                            Count = (ulong)(count - (x + 1)),
                                            UID = (ulong)(count - (x + 1)),
                                        };
                                    }
                                }
                                user.Send(stream.CreateRank(Info));
                            }
                            #endregion

                            #region Dragon
                            else if (ranktyp >= MsgGenericRanking.RankType.LeeLongTotalScore && ranktyp <= MsgGenericRanking.RankType.Suanni)
                            {
                                LeeLongRank.Type key = (LeeLongRank.Type)((uint)ranktyp % 100);
                                LeeLongRank.Entry[] array = LeeLongRank.Ranks[key].RankingList.Values.ToArray<LeeLongRank.Entry>();
                                int num = (int)Page * 10;
                                int offset = Page * max;
                                int count = Math.Min(10, array.Length);
                                var Info = new MsgRankProto()
                                {
                                    Mode = (ulong)Action.Ranking,
                                    ranktyp = (ulong)OldRank,
                                    RegisteredCount = (ulong)array.Length,
                                    Page = Page,
                                    Finish = 1,
                                    Count = (ulong)count,
                                    UID = (ulong)count,
                                };
                                for (byte x = 0; x < count; x++)
                                {
                                    if (x + offset >= array.Length) break;
                                    var entity = array[x + offset];
                                    Info.PlayerList.Add(new MsgRankListPlayers()
                                    {
                                        Rank = (ulong)(offset + x + 1),
                                        Amount = (uint)entity.TotalPoints,
                                        UID1 = entity.UID,
                                        UID2 = entity.UID,
                                        name = entity.Name,
                                        name2 = entity.Name,
                                        Level = entity.Level,
                                        Class = entity.Class,
                                        Mesh = entity.Mesh,
                                        Member10 = 0,
                                        Member11 = 0,
                                        AvatarFrame = 0
                                    });
                                    if (x > 0 && x % 7 == 0 && count > 8)
                                    {
                                        user.Send(stream.CreateRank(Info));
                                        Info = new MsgRankProto()
                                        {
                                            Mode = (ulong)Action.Ranking,
                                            ranktyp = (ulong)OldRank,
                                            RegisteredCount = (ulong)array.Length,
                                            Page = Page,
                                            Finish = 0,
                                            Count = (ulong)(count - (x + 1)),
                                            UID = (ulong)(count - (x + 1)),
                                        };
                                    }
                                }
                                user.Send(stream.CreateRank(Info));
                            }
                            #endregion
                            #region ViodragonClub
                            else if (ranktyp == MsgGenericRanking.RankType.ViodragonClub)
                            {
                                AstredgeRank.Type key = AstredgeRank.Type.ViodragonClub;
                                AstredgeRank.Entry[] array = AstredgeRank.Ranks[key].RankingList.Values.ToArray<AstredgeRank.Entry>();
                                int offset = Page * max;
                                int count = System.Math.Min(max, array.Length);
                                var Info = new MsgRankProto()
                                {
                                    Mode = (ulong)Action.Ranking,
                                    ranktyp = (ulong)OldRank,
                                    RegisteredCount = (ulong)array.Length,
                                    Page = Page,
                                    Finish = 1,
                                    Count = (ulong)count,
                                    UID = (ulong)count,
                                };
                                for (byte x = 0; x < count; x++)
                                {
                                    if (x + offset >= array.Length) break;
                                    var entity = array[x + offset];
                                    Info.PlayerList.Add(new MsgRankListPlayers()
                                    {
                                        Rank = (ulong)(offset + x + 1),
                                        Amount = (uint)entity.TotalPoints,
                                        UID1 = entity.UID,
                                        UID2 = entity.UID,
                                        name = entity.Name,
                                        name2 = entity.Name,
                                        Level = entity.Level,
                                        Class = entity.Class,
                                        Mesh = entity.Mesh,
                                        Member10 = 0,
                                        Member11 = 0,
                                        AvatarFrame = 0
                                    });
                                    if (x > 0 && x % 7 == 0 && count > 8)
                                    {
                                        user.Send(stream.CreateRank(Info));
                                        Info = new MsgRankProto()
                                        {
                                            Mode = (ulong)Action.Ranking,
                                            ranktyp = (ulong)OldRank,
                                            RegisteredCount = (ulong)array.Length,
                                            Page = Page,
                                            Finish = 0,
                                            Count = (ulong)(count - (x + 1)),
                                            UID = (ulong)(count - (x + 1)),
                                        };
                                    }
                                }
                                user.Send(stream.CreateRank(Info));
                            }
                            #endregion
                            #region LoveForEver
                            else if (ranktyp == MsgGenericRanking.RankType.LoveForEver)
                            {
                                AstredgeRank.Type key = AstredgeRank.Type.LoveForEver;
                                AstredgeRank.Entry[] array = AstredgeRank.Ranks[key].RankingList.Values.ToArray<AstredgeRank.Entry>();
                                int offset = Page * max;
                                int count = System.Math.Min(max, array.Length);
                                var Info = new MsgRankProto()
                                {
                                    Mode = (ulong)Action.Ranking,
                                    ranktyp = (ulong)OldRank,
                                    RegisteredCount = (ulong)array.Length,
                                    Page = Page,
                                    Finish = 1,
                                    Count = (ulong)count,
                                    UID = (ulong)count,
                                };
                                for (byte x = 0; x < count; x++)
                                {
                                    if (x + offset >= array.Length) break;
                                    var entity = array[x + offset];
                                    Info.PlayerList.Add(new MsgRankListPlayers()
                                    {
                                        Rank = (ulong)(offset + x + 1),
                                        Amount = (uint)entity.TotalPoints,
                                        UID1 = entity.UID,
                                        UID2 = entity.UID,
                                        name = entity.Name,
                                        name2 = entity.Name,
                                        Level = entity.Level,
                                        Class = entity.Class,
                                        Mesh = entity.Mesh,
                                        Member10 = 0,
                                        Member11 = 0,
                                        AvatarFrame = 0
                                    });
                                    if (x > 0 && x % 7 == 0 && count > 8)
                                    {
                                        user.Send(stream.CreateRank(Info));
                                        Info = new MsgRankProto()
                                        {
                                            Mode = (ulong)Action.Ranking,
                                            ranktyp = (ulong)OldRank,
                                            RegisteredCount = (ulong)array.Length,
                                            Page = Page,
                                            Finish = 0,
                                            Count = (ulong)(count - (x + 1)),
                                            UID = (ulong)(count - (x + 1)),
                                        };
                                    }
                                }
                                user.Send(stream.CreateRank(Info));
                            }
                            #endregion
                            #region HeartLock
                            else if (ranktyp == MsgGenericRanking.RankType.HeartLock)
                            {
                                AstredgeRank.Type key = AstredgeRank.Type.HeartLock;
                                AstredgeRank.Entry[] array = AstredgeRank.Ranks[key].RankingList.Values.ToArray<AstredgeRank.Entry>();
                                int offset = Page * max;
                                int count = System.Math.Min(max, array.Length);
                                var Info = new MsgRankProto()
                                {
                                    Mode = (ulong)Action.Ranking,
                                    ranktyp = (ulong)OldRank,
                                    RegisteredCount = (ulong)array.Length,
                                    Page = Page,
                                    Finish = 1,
                                    Count = (ulong)count,
                                    UID = (ulong)count,
                                };
                                for (byte x = 0; x < count; x++)
                                {
                                    if (x + offset >= array.Length) break;
                                    var entity = array[x + offset];
                                    Info.PlayerList.Add(new MsgRankListPlayers()
                                    {
                                        Rank = (ulong)(offset + x + 1),
                                        Amount = (uint)entity.TotalPoints,
                                        UID1 = entity.UID,
                                        UID2 = entity.UID,
                                        name = entity.Name,
                                        name2 = entity.Name,
                                        Level = entity.Level,
                                        Class = entity.Class,
                                        Mesh = entity.Mesh,
                                        Member10 = 0,
                                        Member11 = 0,
                                        AvatarFrame = 0
                                    });
                                    if (x > 0 && x % 7 == 0 && count > 8)
                                    {
                                        user.Send(stream.CreateRank(Info));
                                        Info = new MsgRankProto()
                                        {
                                            Mode = (ulong)Action.Ranking,
                                            ranktyp = (ulong)OldRank,
                                            RegisteredCount = (ulong)array.Length,
                                            Page = Page,
                                            Finish = 0,
                                            Count = (ulong)(count - (x + 1)),
                                            UID = (ulong)(count - (x + 1)),
                                        };
                                    }
                                }
                                user.Send(stream.CreateRank(Info));
                            }
                            #endregion
                            #region WarlordSpear
                            else if (ranktyp == MsgGenericRanking.RankType.WarlordSpear)
                            {
                                AstredgeRank.Type key = AstredgeRank.Type.WarlordSpear;
                                AstredgeRank.Entry[] array = AstredgeRank.Ranks[key].RankingList.Values.ToArray<AstredgeRank.Entry>();
                                int offset = Page * max;
                                int count = System.Math.Min(max, array.Length);
                                var Info = new MsgRankProto()
                                {
                                    Mode = (ulong)Action.Ranking,
                                    ranktyp = (ulong)OldRank,
                                    RegisteredCount = (ulong)array.Length,
                                    Page = Page,
                                    Finish = 1,
                                    Count = (ulong)count,
                                    UID = (ulong)count,
                                };
                                for (byte x = 0; x < count; x++)
                                {
                                    if (x + offset >= array.Length) break;
                                    var entity = array[x + offset];
                                    Info.PlayerList.Add(new MsgRankListPlayers()
                                    {
                                        Rank = (ulong)(offset + x + 1),
                                        Amount = (uint)entity.TotalPoints,
                                        UID1 = entity.UID,
                                        UID2 = entity.UID,
                                        name = entity.Name,
                                        name2 = entity.Name,
                                        Level = entity.Level,
                                        Class = entity.Class,
                                        Mesh = entity.Mesh,
                                        Member10 = 0,
                                        Member11 = 0,
                                        AvatarFrame = 0
                                    });
                                    if (x > 0 && x % 7 == 0 && count > 8)
                                    {
                                        user.Send(stream.CreateRank(Info));
                                        Info = new MsgRankProto()
                                        {
                                            Mode = (ulong)Action.Ranking,
                                            ranktyp = (ulong)OldRank,
                                            RegisteredCount = (ulong)array.Length,
                                            Page = Page,
                                            Finish = 0,
                                            Count = (ulong)(count - (x + 1)),
                                            UID = (ulong)(count - (x + 1)),
                                        };
                                    }
                                }
                                user.Send(stream.CreateRank(Info));
                            }
                            #endregion
                            #region InnerPower
                            else if (ranktyp == RankType.InnerPower)
                            {
                                var array = Role.Instance.InnerPower.InnerPowerRank.GetRankingList();
                                int offset = Page * max;
                                int count = System.Math.Min(max, array.Length);
                                var Info = new MsgRankProto()
                                {
                                    Mode = (ulong)Action.Ranking,
                                    ranktyp = (ulong)OldRank,
                                    RegisteredCount = (ulong)Role.Instance.InnerPower.InnerPowerRank.MaxPlayers,
                                    Page = Page,
                                    Finish = 1,
                                    Count = (ulong)count,
                                    UID = (ulong)count,
                                };
                                for (byte x = 0; x < count; x++)
                                {
                                    if (x + offset >= array.Length) break;
                                    var entity = array[x + offset];
                                    Info.PlayerList.Add(new MsgRankListPlayers()
                                    {
                                        Rank = (ulong)(offset + x + 1),
                                        Amount = (uint)entity.TotalScore,
                                        UID1 = entity.UID,
                                        UID2 = entity.UID,
                                        name = entity.Name,
                                        name2 = entity.Name,
                                        Level = 0,
                                        Class = 0,
                                        Mesh = 0,
                                        Member10 = 0,
                                        Member11 = 0,
                                        AvatarFrame = 0
                                    });
                                    if (x > 0 && x % 7 == 0 && count > 8)
                                    {
                                        user.Send(stream.CreateRank(Info));
                                        Info = new MsgRankProto()
                                        {
                                            Mode = (ulong)Action.Ranking,
                                            ranktyp = (ulong)OldRank,
                                            RegisteredCount = (ulong)array.Length,
                                            Page = Page,
                                            Finish = 0,
                                            Count = (ulong)(count - (x + 1)),
                                            UID = (ulong)(count - (x + 1)),
                                        };
                                    }
                                }
                                user.Send(stream.CreateRank(Info));
                                for (int x = 0; x < array.Length; x++)
                                {
                                    if (array[x].UID == user.Player.UID)
                                    {
                                        Info = new MsgRankProto()
                                        {
                                            Mode = (ulong)Action.QueryCount,
                                            ranktyp = (ulong)RankType.InnerPower,
                                            RegisteredCount = (ulong)0,
                                            Page = 0,
                                            Finish = 1,
                                        };
                                        Info.PlayerList.Add(new MsgRankListPlayers()
                                        {
                                            Rank = (ulong)(x + 1),
                                            Amount = (uint)array[x].TotalScore,
                                            UID1 = array[x].UID,
                                            UID2 = array[x].UID,
                                            name = array[x].Name,
                                            name2 = array[x].Name,
                                            Level = 0,
                                            Class = 0,
                                            Mesh = 0,
                                            Member10 = 0,
                                            Member11 = 0,
                                            AvatarFrame = 0
                                        });
                                        user.Send(stream.CreateRank(Info));
                                        break;
                                    }
                                }
                            }
                            #endregion
                            #region YuanshenRank
                            else if (ranktyp == MsgGenericRanking.RankType.YuanshenRank)
                            {
                                YuanshenRank.Type key = (YuanshenRank.Type)((uint)ranktyp % 100);
                                YuanshenRank.Entry[] array = YuanshenRank.Ranks[key].RankingList.Values.ToArray<YuanshenRank.Entry>();
                                int offset = Page * max;
                                int count = System.Math.Min(max, array.Length);
                                var Info = new MsgRankProto()
                                {
                                    Mode = (ulong)Action.Ranking,
                                    ranktyp = (ulong)OldRank,
                                    RegisteredCount = (ulong)array.Length,
                                    Page = Page,
                                    Finish = 1,
                                    Count = (ulong)count,
                                    UID = (ulong)count,
                                };
                                for (byte x = 0; x < count; x++)
                                {
                                    if (x + offset >= array.Length) break;
                                    var entity = array[x + offset];
                                    Info.PlayerList.Add(new MsgRankListPlayers()
                                    {
                                        Rank = (ulong)(offset + x + 1),
                                        Amount = (uint)entity.TotalPoints,
                                        UID1 = entity.UID,
                                        UID2 = entity.UID,
                                        name = entity.Name,
                                        name2 = entity.Name,
                                        Level = entity.Level,
                                        Class = entity.Class,
                                        Mesh = entity.Mesh,
                                        Member10 = 0,
                                        Member11 = 0,
                                        AvatarFrame = 0
                                    });
                                    if (x > 0 && x % 7 == 0 && count > 8)
                                    {
                                        user.Send(stream.CreateRank(Info));
                                        Info = new MsgRankProto()
                                        {
                                            Mode = (ulong)Action.Ranking,
                                            ranktyp = (ulong)OldRank,
                                            RegisteredCount = (ulong)array.Length,
                                            Page = Page,
                                            Finish = 0,
                                            Count = (ulong)(count - (x + 1)),
                                            UID = (ulong)(count - (x + 1)),
                                        };
                                    }
                                }
                                user.Send(stream.CreateRank(Info));
                            }
                            #endregion
                            #region Chi
                            else if (ranktyp >= RankType.DragonChi && ranktyp <= RankType.TurtleChi)
                            {
                                Role.Instance.Chi.ChiPower[] Powers = null;
                                if (ranktyp == RankType.DragonChi)
                                    Powers = Pool.ChiRanking.Dragon.Values.ToArray();
                                else if (ranktyp == RankType.PhoenixChi)
                                    Powers = Pool.ChiRanking.Phoenix.Values.ToArray();
                                else if (ranktyp == RankType.TigerChi)
                                    Powers = Pool.ChiRanking.Tiger.Values.ToArray();
                                else if (ranktyp == RankType.TurtleChi)
                                    Powers = Pool.ChiRanking.Turtle.Values.ToArray();

                                if (Powers == null) return;

                                int offset = Page * max;
                                int count = Math.Min(max, Powers.Length);

                                var Info = new MsgRankProto()
                                {
                                    Mode = (ulong)Action.Ranking,
                                    ranktyp = (ulong)OldRank,
                                    RegisteredCount = (ulong)Role.Instance.ChiRank.File_Size,
                                    Page = Page,
                                    Finish = 1,
                                    Count = (ulong)count,
                                    UID = (ulong)count,
                                };
                                for (byte x = 0; x < count; x++)
                                {
                                    if (x + offset >= Powers.Length) break;
                                    var entity = Powers[x + offset];
                                    Info.PlayerList.Add(new MsgRankListPlayers()
                                    {
                                        Rank = (ulong)(entity.Rank),
                                        Amount = (uint)entity.Score,
                                        UID1 = entity.UID,
                                        UID2 = entity.UID,
                                        name = entity.Name,
                                        name2 = entity.Name,
                                        Level = 0,
                                        Class = 0,
                                        Mesh = 0,
                                        Member10 = 0,
                                        Member11 = 0,
                                        AvatarFrame = 0
                                    });
                                    if (x > 0 && x % 7 == 0 && count > 8)
                                    {
                                        user.Send(stream.CreateRank(Info));
                                        Info = new MsgRankProto()
                                        {
                                            Mode = (ulong)Action.Ranking,
                                            ranktyp = (ulong)OldRank,
                                            RegisteredCount = (ulong)Role.Instance.ChiRank.File_Size,
                                            Page = Page,
                                            Finish = 0,
                                            Count = (ulong)(count - (x + 1)),
                                            UID = (ulong)(count - (x + 1)),
                                        };
                                    }
                                }
                                user.Send(stream.CreateRank(Info));
                            }
                            #endregion

                            #region Rune
                            else if (ranktyp == RankType.Rune)
                            {
                                var array = Database.RuneRank.RankingList.ToArray();
                                int offset = Page * max;
                                int count = System.Math.Min(max, array.Length);
                                var Info = new MsgRankProto()
                                {
                                    Mode = (ulong)Action.Ranking,
                                    ranktyp = (ulong)OldRank,
                                    RegisteredCount = (ulong)Database.RuneRank.MaxPlayers,
                                    Page = Page,
                                    Finish = 1,
                                    Count = (ulong)count,
                                    UID = (ulong)count,
                                };
                                for (byte x = 0; x < count; x++)
                                {
                                    if (x + offset >= array.Length) break;
                                    var entity = array[x + offset];
                                    Info.PlayerList.Add(new MsgRankListPlayers()
                                    {
                                        Rank = (ulong)(offset + x + 1),
                                        Amount = (uint)entity.Value.TotalPoints,
                                        UID1 = entity.Key,
                                        UID2 = entity.Key,
                                        name = entity.Value.Name,
                                        name2 = entity.Value.Name,
                                        Level = 0,
                                        Class = 0,
                                        Mesh = 0,
                                        Member10 = 0,
                                        Member11 = 0,
                                        AvatarFrame = 0
                                    });
                                    if (x > 0 && x % 7 == 0 && count > 8)
                                    {
                                        user.Send(stream.CreateRank(Info));
                                        Info = new MsgRankProto()
                                        {
                                            Mode = (ulong)Action.Ranking,
                                            ranktyp = (ulong)OldRank,
                                            RegisteredCount = (ulong)Database.RuneRank.MaxPlayers,
                                            Page = Page,
                                            Finish = 0,
                                            Count = (ulong)(count - (x + 1)),
                                            UID = (ulong)(count - (x + 1)),
                                        };
                                    }
                                }
                                user.Send(stream.CreateRank(Info));
                                for (int x = 0; x < array.Length; x++)
                                {
                                    if (array[x].Key == user.Player.UID)
                                    {
                                        Info = new MsgRankProto()
                                        {
                                            Mode = (ulong)Action.QueryCount,
                                            ranktyp = (ulong)RankType.Rune,
                                            RegisteredCount = (ulong)0,
                                            Page = 0,
                                            Finish = 1,
                                        };
                                        Info.PlayerList.Add(new MsgRankListPlayers()
                                        {
                                            Rank = (ulong)(x + 1),
                                            Amount = (uint)array[x].Value.TotalPoints,
                                            UID1 = array[x].Key,
                                            UID2 = array[x].Key,
                                            name = array[x].Value.Name,
                                            name2 = array[x].Value.Name,
                                            Level = 0,
                                            Class = 0,
                                            Mesh = 0,
                                            Member10 = 0,
                                            Member11 = 0,
                                            AvatarFrame = 0
                                        });
                                        user.Send(stream.CreateRank(Info));
                                        break;
                                    }
                                }
                            }
                            #endregion

                            #region RoseRank
                            else if (ranktyp >= RankType.RoseFairy && ranktyp <= RankType.TulipFairy)
                            {
                                Role.Instance.Flowers.Flower[] Powers = null;
                                if (ranktyp == RankType.RoseFairy)
                                    Powers = Pool.GirlsFlowersRanking.RedRoses.Values.Where(p => p.Amount > 0).ToArray();
                                else if (ranktyp == RankType.OrchidFairy)
                                    Powers = Pool.GirlsFlowersRanking.Orchids.Values.Where(p => p.Amount > 0).ToArray();
                                else if (ranktyp == RankType.LilyFairy)
                                    Powers = Pool.GirlsFlowersRanking.Lilies.Values.Where(p => p.Amount > 0).ToArray();
                                else if (ranktyp == RankType.TulipFairy)
                                    Powers = Pool.GirlsFlowersRanking.Tulips.Values.Where(p => p.Amount > 0).ToArray();

                                if (Powers == null) return;
                                int offset = Page * max;
                                int count = System.Math.Min(max, Powers.Length);
                                var Info = new MsgRankProto()
                                {
                                    Mode = (ulong)Action.Ranking,
                                    ranktyp = (ulong)OldRank,
                                    RegisteredCount = (ulong)Powers.Length,
                                    Page = Page,
                                    Finish = 1,
                                    Count = (ulong)count,
                                    UID = (ulong)count,
                                };
                                for (byte x = 0; x < count; x++)
                                {
                                    if (x + offset >= Powers.Length) break;
                                    var entity = Powers[x + offset];
                                    Info.PlayerList.Add(new MsgRankListPlayers()
                                    {
                                        Rank = (ulong)offset + x + 1,
                                        Amount = entity.Amount,
                                        UID1 = entity.UID,
                                        UID2 = entity.UID,
                                        name = entity.Name,
                                        name2 = entity.Name,
                                        Level = 0,
                                        Class = 0,
                                        Mesh = entity.Mesh,
                                        Member10 = 0,
                                        Member11 = 0,
                                        AvatarFrame = 0,
                                    });
                                    if (x > 0 && x % 7 == 0 && count > 8)
                                    {
                                        user.Send(stream.CreateRank(Info));
                                        Info = new MsgRankProto()
                                        {
                                            Mode = (ulong)Action.Ranking,
                                            ranktyp = (ulong)OldRank,
                                            RegisteredCount = (ulong)Powers.Length,
                                            Page = Page,
                                            Finish = 0,
                                            Count = (ulong)(count - (x + 1)),
                                            UID = (ulong)(count - (x + 1)),
                                        };
                                    }
                                }
                                user.Send(stream.CreateRank(Info));
                            }
                            else if (ranktyp >= RankType.GirlsTotalFlower && ranktyp <= RankType.GirlsTotalFlower)
                            {
                                Role.Instance.Flowers.Flower[] Powers = null;
                                //if (ranktyp == RankType.GirlsTotalFlower)
                                //    Powers = Pool.GirlsFlowersRanking.GirlALLFlower.Values.Where(p => p.Amount > 0).ToArray();
                                // total tab = Peonies list (same data as TaskRank 50025)
                                Powers = Pool.GirlsFlowersRanking.Peonies.Values.Where(p => p.Amount > 0).ToArray();

                                if (Powers == null) return;
                                int offset = Page * max;
                                int count = System.Math.Min(max, Powers.Length);
                                var Info = new MsgRankProto()
                                {
                                    Mode = (ulong)Action.Ranking,
                                    ranktyp = (ulong)OldRank,
                                    RegisteredCount = (ulong)Powers.Length,
                                    Page = Page,
                                    Finish = 1,
                                    Count = (ulong)count,
                                    UID = (ulong)count,
                                };
                                for (byte x = 0; x < count; x++)
                                {
                                    if (x + offset >= Powers.Length) break;
                                    var entity = Powers[x + offset];
                                    Info.PlayerList.Add(new MsgRankListPlayers()
                                    {
                                        Rank = (ulong)offset + x + 1,
                                        Amount = entity.Amount,
                                        UID1 = entity.UID,
                                        UID2 = entity.UID,
                                        name = entity.Name,
                                        name2 = entity.Name,
                                        Level = 0,
                                        Class = 0,
                                        Mesh = entity.Mesh,
                                        Member10 = 0,
                                        Member11 = 0,
                                        AvatarFrame = user.Player.FrameID,
                                    });
                                    if (x > 0 && x % 7 == 0 && count > 8)
                                    {
                                        user.Send(stream.CreateRank(Info));
                                        Info = new MsgRankProto()
                                        {
                                            Mode = (ulong)Action.Ranking,
                                            ranktyp = (ulong)OldRank,
                                            RegisteredCount = (ulong)Powers.Length,
                                            Page = Page,
                                            Finish = 0,
                                            Count = (ulong)(count - (x + 1)),
                                            UID = (ulong)(count - (x + 1)),
                                        };
                                    }
                                }
                                user.Send(stream.CreateRank(Info));
                            }
                            else if (ranktyp >= RankType.Kiss && ranktyp <= RankType.Jade)
                            {

                                Role.Instance.Flowers.Flower[] Powers = null;
                                if (ranktyp == RankType.Kiss)
                                    Powers = Pool.BoysFlowersRanking.RedRoses.Values.ToArray();
                                else if (ranktyp == RankType.Tins)
                                    Powers = Pool.BoysFlowersRanking.Orchids.Values.ToArray();
                                else if (ranktyp == RankType.Love)
                                    Powers = Pool.BoysFlowersRanking.Lilies.Values.ToArray();
                                else if (ranktyp == RankType.Jade)
                                    Powers = Pool.BoysFlowersRanking.Tulips.Values.ToArray();

                                if (Powers == null) return;
                                int offset = Page * max;
                                int count = Math.Min(max, Powers.Length);
                                var Info = new MsgRankProto()
                                {
                                    Mode = (ulong)Action.Ranking,
                                    ranktyp = (ulong)OldRank,
                                    RegisteredCount = (ulong)Powers.Length,
                                    Page = Page,
                                    Finish = 1,
                                    Count = (ulong)count,
                                    UID = (ulong)count,
                                };
                                for (byte x = 0; x < count; x++)
                                {
                                    if (x + offset >= Powers.Length) break;
                                    var entity = Powers[x + offset];
                                    Info.PlayerList.Add(new MsgRankListPlayers()
                                    {
                                        Rank = (ulong)offset + x + 1,
                                        Amount = entity.Amount,
                                        UID1 = entity.UID,
                                        UID2 = entity.UID,
                                        name = entity.Name,
                                        name2 = entity.Name,
                                        Level = 0,
                                        Class = 0,
                                        Mesh = entity.Mesh,
                                        Member10 = 0,
                                        Member11 = 0,
                                        AvatarFrame = 0,
                                    });
                                    if (x > 0 && x % 7 == 0 && count > 8)
                                    {
                                        user.Send(stream.CreateRank(Info));
                                        Info = new MsgRankProto()
                                        {
                                            Mode = (ulong)Action.Ranking,
                                            ranktyp = (ulong)OldRank,
                                            RegisteredCount = (ulong)Powers.Length,
                                            Page = Page,
                                            Finish = 0,
                                            Count = (ulong)(count - (x + 1)),
                                            UID = (ulong)(count - (x + 1)),
                                        };
                                    }
                                }
                                user.Send(stream.CreateRank(Info));
                            }
                            else if (ranktyp >= RankType.BoysTotalFlower && ranktyp <= RankType.BoysTotalFlower)
                            {

                                Role.Instance.Flowers.Flower[] Powers = null;
                                //if (ranktyp == RankType.BoysTotalFlower)
                                //    Powers = Pool.BoysFlowersRanking.GirlALLFlower.Values.ToArray();
                                // total tab = Peonies list (same data as TaskRank 50026)
                                Powers = Pool.BoysFlowersRanking.Peonies.Values.Where(p => p.Amount > 0).ToArray();

                                if (Powers == null) return;
                                int offset = Page * max;
                                int count = Math.Min(max, Powers.Length);
                                var Info = new MsgRankProto()
                                {
                                    Mode = (ulong)Action.Ranking,
                                    ranktyp = (ulong)OldRank,
                                    RegisteredCount = (ulong)Powers.Length,
                                    Page = Page,
                                    Finish = 1,
                                    Count = (ulong)count,
                                    UID = (ulong)count,
                                };
                                for (byte x = 0; x < count; x++)
                                {
                                    if (x + offset >= Powers.Length) break;
                                    var entity = Powers[x + offset];
                                    Info.PlayerList.Add(new MsgRankListPlayers()
                                    {
                                        Rank = (ulong)offset + x + 1,
                                        Amount = entity.Amount,
                                        UID1 = entity.UID,
                                        UID2 = entity.UID,
                                        name = entity.Name,
                                        name2 = entity.Name,
                                        Level = 0,
                                        Class = 0,
                                        Mesh = entity.Mesh,
                                        Member10 = 0,
                                        Member11 = 0,
                                        AvatarFrame = user.Player.FrameID,
                                    });
                                    if (x > 0 && x % 7 == 0 && count > 8)
                                    {
                                        user.Send(stream.CreateRank(Info));
                                        Info = new MsgRankProto()
                                        {
                                            Mode = (ulong)Action.Ranking,
                                            ranktyp = (ulong)OldRank,
                                            RegisteredCount = (ulong)Powers.Length,
                                            Page = Page,
                                            Finish = 0,
                                            Count = (ulong)(count - (x + 1)),
                                            UID = (ulong)(count - (x + 1)),
                                        };
                                    }
                                }
                                user.Send(stream.CreateRank(Info));
                            }
                            #endregion
                            break;
                        }
                    case Action.WeeklyRankings:// client: 9 = weekly list
                        {
                            var OldRank = ranktyp;
                            #region Archer
                            if (ranktyp == MsgGenericRanking.RankType.WeeklyArcher)
                            {
                                ArcherRank.Type key = ArcherRank.Type.WeeklyArcher;
                                ArcherRank.Entry[] array = ArcherRank.Ranks[key].RankingList.Values.ToArray<ArcherRank.Entry>();
                                int offset = Page * max;
                                int count = System.Math.Min(max, array.Length);
                                var Info = new MsgRankProto()
                                {
                                    Mode = (ulong)Action.WeeklyRankings,
                                    ranktyp = (ulong)OldRank,
                                    RegisteredCount = (ulong)array.Length,
                                    Page = Page,
                                    Finish = 1,
                                    Count = (ulong)count,
                                    UID = (ulong)count,
                                };
                                for (byte x = 0; x < count; x++)
                                {
                                    if (x + offset >= array.Length) break;
                                    var entity = array[x + offset];
                                    Info.PlayerList.Add(new MsgRankListPlayers()
                                    {
                                        Rank = (ulong)(offset + x + 1),
                                        Amount = (uint)entity.TotalPoints,
                                        UID1 = entity.UID,
                                        UID2 = entity.UID,
                                        name = entity.Name,
                                        name2 = entity.Name,
                                        Level = entity.Level,
                                        Class = entity.Class,
                                        Mesh = entity.Mesh,
                                        Member10 = 0,
                                        Member11 = 0,
                                        AvatarFrame = 0
                                    });
                                    if (x > 0 && x % 7 == 0 && count > 8)
                                    {
                                        user.Send(stream.CreateRank(Info));
                                        Info = new MsgRankProto()
                                        {
                                            Mode = (ulong)Action.WeeklyRankings,
                                            ranktyp = (ulong)OldRank,
                                            RegisteredCount = (ulong)array.Length,
                                            Page = Page,
                                            Finish = 0,
                                            Count = (ulong)(count - (x + 1)),
                                            UID = (ulong)(count - (x + 1)),
                                        };
                                    }
                                }
                                user.Send(stream.CreateRank(Info));
                            }
                            #endregion
                            #region MonkWeeklyRanking
                            if (ranktyp == MsgGenericRanking.RankType.MonkWeeklyRanking)
                            {
                                MonkRanks.Type key = MonkRanks.Type.WeeklyMonk;
                                MonkRanks.Entry[] array = MonkRanks.Ranks[key].RankingList.Values.ToArray<MonkRanks.Entry>();
                                int offset = Page * max;
                                int count = System.Math.Min(max, array.Length);
                                var Info = new MsgRankProto()
                                {
                                    Mode = (ulong)Action.WeeklyRankings,
                                    ranktyp = (ulong)OldRank,
                                    RegisteredCount = (ulong)array.Length,
                                    Page = Page,
                                    Finish = 1,
                                    Count = (ulong)count,
                                    UID = (ulong)count,
                                };
                                for (byte x = 0; x < count; x++)
                                {
                                    if (x + offset >= array.Length) break;
                                    var entity = array[x + offset];
                                    Info.PlayerList.Add(new MsgRankListPlayers()
                                    {
                                        Rank = (ulong)(offset + x + 1),
                                        Amount = (uint)entity.TotalPoints,
                                        UID1 = entity.UID,
                                        UID2 = entity.UID,
                                        name = entity.Name,
                                        name2 = entity.Name,
                                        Level = entity.Level,
                                        Class = entity.Class,
                                        Mesh = entity.Mesh,
                                        Member10 = 0,
                                        Member11 = 0,
                                        AvatarFrame = 0
                                    });
                                    if (x > 0 && x % 7 == 0 && count > 8)
                                    {
                                        user.Send(stream.CreateRank(Info));
                                        Info = new MsgRankProto()
                                        {
                                            Mode = (ulong)Action.WeeklyRankings,
                                            ranktyp = (ulong)OldRank,
                                            RegisteredCount = (ulong)array.Length,
                                            Page = Page,
                                            Finish = 0,
                                            Count = (ulong)(count - (x + 1)),
                                            UID = (ulong)(count - (x + 1)),
                                        };
                                    }
                                }
                                user.Send(stream.CreateRank(Info));
                            }
                            #endregion
                            #region TaoRank
                            else if (ranktyp == MsgGenericRanking.RankType.WeeklyTao && !AtributesStatus.IsFire(user.Player.Class))// any viewer; Fire class sees Fire list
                            {
                                WaterRank.Type key = WaterRank.Type.WeeklyTao;
                                WaterRank.Entry[] array = WaterRank.Ranks[key].RankingList.Values.ToArray<WaterRank.Entry>();
                                int offset = Page * max;
                                int count = System.Math.Min(max, array.Length);
                                var Info = new MsgRankProto()
                                {
                                    Mode = (ulong)Action.WeeklyRankings,
                                    ranktyp = (ulong)OldRank,
                                    RegisteredCount = (ulong)array.Length,
                                    Page = Page,
                                    Finish = 1,
                                    Count = (ulong)count,
                                    UID = (ulong)count,
                                };
                                for (byte x = 0; x < count; x++)
                                {
                                    if (x + offset >= array.Length) break;
                                    var entity = array[x + offset];
                                    Info.PlayerList.Add(new MsgRankListPlayers()
                                    {
                                        Rank = (ulong)(offset + x + 1),
                                        Amount = (uint)entity.TotalPoints,
                                        UID1 = entity.UID,
                                        UID2 = entity.UID,
                                        name = entity.Name,
                                        name2 = entity.Name,
                                        Level = entity.Level,
                                        Class = entity.Class,
                                        Mesh = entity.Mesh,
                                        Member10 = 0,
                                        Member11 = 0,
                                        AvatarFrame = 0
                                    });
                                    if (x > 0 && x % 7 == 0 && count > 8)
                                    {
                                        user.Send(stream.CreateRank(Info));
                                        Info = new MsgRankProto()
                                        {
                                            Mode = (ulong)Action.WeeklyRankings,
                                            ranktyp = (ulong)OldRank,
                                            RegisteredCount = (ulong)array.Length,
                                            Page = Page,
                                            Finish = 0,
                                            Count = (ulong)(count - (x + 1)),
                                            UID = (ulong)(count - (x + 1)),
                                        };
                                    }
                                }
                                user.Send(stream.CreateRank(Info));
                            }
                            else if (ranktyp == MsgGenericRanking.RankType.WeeklyTao && AtributesStatus.IsFire(user.Player.Class))
                            {
                                FireRank.Type key = FireRank.Type.WeeklyTao;
                                FireRank.Entry[] array = FireRank.Ranks[key].RankingList.Values.ToArray<FireRank.Entry>();
                                int offset = Page * max;
                                int count = System.Math.Min(max, array.Length);
                                var Info = new MsgRankProto()
                                {
                                    Mode = (ulong)Action.WeeklyRankings,
                                    ranktyp = (ulong)OldRank,
                                    RegisteredCount = (ulong)array.Length,
                                    Page = Page,
                                    Finish = 1,
                                    Count = (ulong)count,
                                    UID = (ulong)count,
                                };
                                for (byte x = 0; x < count; x++)
                                {
                                    if (x + offset >= array.Length) break;
                                    var entity = array[x + offset];
                                    Info.PlayerList.Add(new MsgRankListPlayers()
                                    {
                                        Rank = (ulong)(offset + x + 1),
                                        Amount = (uint)entity.TotalPoints,
                                        UID1 = entity.UID,
                                        UID2 = entity.UID,
                                        name = entity.Name,
                                        name2 = entity.Name,
                                        Level = entity.Level,
                                        Class = entity.Class,
                                        Mesh = entity.Mesh,
                                        Member10 = 0,
                                        Member11 = 0,
                                        AvatarFrame = 0
                                    });
                                    if (x > 0 && x % 7 == 0 && count > 8)
                                    {
                                        user.Send(stream.CreateRank(Info));
                                        Info = new MsgRankProto()
                                        {
                                            Mode = (ulong)Action.WeeklyRankings,
                                            ranktyp = (ulong)OldRank,
                                            RegisteredCount = (ulong)array.Length,
                                            Page = Page,
                                            Finish = 0,
                                            Count = (ulong)(count - (x + 1)),
                                            UID = (ulong)(count - (x + 1)),
                                        };
                                    }
                                }
                                user.Send(stream.CreateRank(Info));
                            }
                            #endregion
                            #region PirateRank
                            else if (ranktyp == MsgGenericRanking.RankType.WeeklyPirate)
                            {
                                PirateRank.Type key = PirateRank.Type.WeeklyPirate;
                                PirateRank.Entry[] array = PirateRank.Ranks[key].RankingList.Values.ToArray<PirateRank.Entry>();
                                int offset = Page * max;
                                int count = System.Math.Min(max, array.Length);
                                var Info = new MsgRankProto()
                                {
                                    Mode = (ulong)Action.WeeklyRankings,
                                    ranktyp = (ulong)OldRank,
                                    RegisteredCount = (ulong)array.Length,
                                    Page = Page,
                                    Finish = 1,
                                    Count = (ulong)count,
                                    UID = (ulong)count,
                                };
                                for (byte x = 0; x < count; x++)
                                {
                                    if (x + offset >= array.Length) break;
                                    var entity = array[x + offset];
                                    Info.PlayerList.Add(new MsgRankListPlayers()
                                    {
                                        Rank = (ulong)(offset + x + 1),
                                        Amount = (uint)entity.TotalPoints,
                                        UID1 = entity.UID,
                                        UID2 = entity.UID,
                                        name = entity.Name,
                                        name2 = entity.Name,
                                        Level = entity.Level,
                                        Class = entity.Class,
                                        Mesh = entity.Mesh,
                                        Member10 = 0,
                                        Member11 = 0,
                                        AvatarFrame = 0
                                    });
                                    if (x > 0 && x % 7 == 0 && count > 8)
                                    {
                                        user.Send(stream.CreateRank(Info));
                                        Info = new MsgRankProto()
                                        {
                                            Mode = (ulong)Action.WeeklyRankings,
                                            ranktyp = (ulong)OldRank,
                                            RegisteredCount = (ulong)array.Length,
                                            Page = Page,
                                            Finish = 0,
                                            Count = (ulong)(count - (x + 1)),
                                            UID = (ulong)(count - (x + 1)),
                                        };
                                    }
                                }
                                user.Send(stream.CreateRank(Info));
                            }
                            #endregion
                            #region Dragon
                            if (ranktyp == MsgGenericRanking.RankType.LeeLongWeekly)
                            {
                                LeeLongRank.Type key = LeeLongRank.Type.LeeLongWeekly;
                                LeeLongRank.Entry[] array = LeeLongRank.Ranks[key].RankingList.Values.ToArray<LeeLongRank.Entry>();
                                int offset = Page * max;
                                int count = System.Math.Min(max, array.Length);
                                var Info = new MsgRankProto()
                                {
                                    Mode = (ulong)Action.WeeklyRankings,
                                    ranktyp = (ulong)OldRank,
                                    RegisteredCount = (ulong)array.Length,
                                    Page = Page,
                                    Finish = 1,
                                    Count = (ulong)count,
                                    UID = (ulong)count,
                                };
                                for (byte x = 0; x < count; x++)
                                {
                                    if (x + offset >= array.Length) break;
                                    var entity = array[x + offset];
                                    Info.PlayerList.Add(new MsgRankListPlayers()
                                    {
                                        Rank = (ulong)(offset + x + 1),
                                        Amount = (uint)entity.TotalPoints,
                                        UID1 = entity.UID,
                                        UID2 = entity.UID,
                                        name = entity.Name,
                                        name2 = entity.Name,
                                        Level = entity.Level,
                                        Class = entity.Class,
                                        Mesh = entity.Mesh,
                                        Member10 = 0,
                                        Member11 = 0,
                                        AvatarFrame = 0
                                    });
                                    if (x > 0 && x % 7 == 0 && count > 8)
                                    {
                                        user.Send(stream.CreateRank(Info));
                                        Info = new MsgRankProto()
                                        {
                                            Mode = (ulong)Action.WeeklyRankings,
                                            ranktyp = (ulong)OldRank,
                                            RegisteredCount = (ulong)array.Length,
                                            Page = Page,
                                            Finish = 0,
                                            Count = (ulong)(count - (x + 1)),
                                            UID = (ulong)(count - (x + 1)),
                                        };
                                    }
                                }
                                user.Send(stream.CreateRank(Info));
                            }
                            #endregion
                            #region ViodragonClubWeekly
                            if (ranktyp == MsgGenericRanking.RankType.ViodragonClubWeekly)
                            {
                                AstredgeRank.Type key = AstredgeRank.Type.AstredgeWeekly;
                                AstredgeRank.Entry[] array = AstredgeRank.Ranks[key].RankingList.Values.ToArray<AstredgeRank.Entry>();
                                int offset = Page * max;
                                int count = System.Math.Min(max, array.Length);
                                var Info = new MsgRankProto()
                                {
                                    Mode = (ulong)Action.WeeklyRankings,
                                    ranktyp = (ulong)OldRank,
                                    RegisteredCount = (ulong)array.Length,
                                    Page = Page,
                                    Finish = 1,
                                    Count = (ulong)count,
                                    UID = (ulong)count,
                                };
                                for (byte x = 0; x < count; x++)
                                {
                                    if (x + offset >= array.Length) break;
                                    var entity = array[x + offset];
                                    Info.PlayerList.Add(new MsgRankListPlayers()
                                    {
                                        Rank = (ulong)(offset + x + 1),
                                        Amount = (uint)entity.TotalPoints,
                                        UID1 = entity.UID,
                                        UID2 = entity.UID,
                                        name = entity.Name,
                                        name2 = entity.Name,
                                        Level = entity.Level,
                                        Class = entity.Class,
                                        Mesh = entity.Mesh,
                                        Member10 = 0,
                                        Member11 = 0,
                                        AvatarFrame = 0
                                    });
                                    if (x > 0 && x % 7 == 0 && count > 8)
                                    {
                                        user.Send(stream.CreateRank(Info));
                                        Info = new MsgRankProto()
                                        {
                                            Mode = (ulong)Action.WeeklyRankings,
                                            ranktyp = (ulong)OldRank,
                                            RegisteredCount = (ulong)array.Length,
                                            Page = Page,
                                            Finish = 0,
                                            Count = (ulong)(count - (x + 1)),
                                            UID = (ulong)(count - (x + 1)),
                                        };
                                    }
                                }
                                user.Send(stream.CreateRank(Info));
                            }
                            #endregion
                            #region YuanshenRank
                            if (ranktyp == MsgGenericRanking.RankType.YuanshenRank)
                            {
                                YuanshenRank.Type key = (YuanshenRank.Type)((uint)ranktyp % 100);
                                YuanshenRank.Entry[] array = YuanshenRank.Ranks[key].RankingList.Values.ToArray<YuanshenRank.Entry>();
                                int offset = Page * max;
                                int count = System.Math.Min(max, array.Length);
                                var Info = new MsgRankProto()
                                {
                                    Mode = (ulong)Action.WeeklyRankings,
                                    ranktyp = (ulong)OldRank,
                                    RegisteredCount = (ulong)array.Length,
                                    Page = Page,
                                    Finish = 1,
                                    Count = (ulong)count,
                                    UID = (ulong)count,
                                };
                                for (byte x = 0; x < count; x++)
                                {
                                    if (x + offset >= array.Length) break;
                                    var entity = array[x + offset];
                                    Info.PlayerList.Add(new MsgRankListPlayers()
                                    {
                                        Rank = (ulong)(offset + x + 1),
                                        Amount = (uint)entity.TotalPoints,
                                        UID1 = entity.UID,
                                        UID2 = entity.UID,
                                        name = entity.Name,
                                        name2 = entity.Name,
                                        Level = entity.Level,
                                        Class = entity.Class,
                                        Mesh = entity.Mesh,
                                        Member10 = 0,
                                        Member11 = 0,
                                        AvatarFrame = 0
                                    });
                                    if (x > 0 && x % 7 == 0 && count > 8)
                                    {
                                        user.Send(stream.CreateRank(Info));
                                        Info = new MsgRankProto()
                                        {
                                            Mode = (ulong)Action.WeeklyRankings,
                                            ranktyp = (ulong)OldRank,
                                            RegisteredCount = (ulong)array.Length,
                                            Page = Page,
                                            Finish = 0,
                                            Count = (ulong)(count - (x + 1)),
                                            UID = (ulong)(count - (x + 1)),
                                        };
                                    }
                                }
                                user.Send(stream.CreateRank(Info));
                            }
                            #endregion
                            break;
                        }
                    case Action.TotalPoints:// client: 8 = my weekly rank
                        {
                            var OldRank = ranktyp;
                            #region Archer
                            if (ranktyp == MsgGenericRanking.RankType.WeeklyArcher)
                            {
                                uint myRank = ArcherRank.GetMyRank(ArcherRank.Type.WeeklyArcher, user.Player.UID);
                                var info = ArcherRank.GetInfo(ArcherRank.Type.WeeklyArcher, user.Player.UID);
                                var proto = new MsgRankProto()
                                {
                                    Mode = (ulong)Action.TotalPoints,
                                    ranktyp = (ulong)ranktyp,
                                    Page = 0,
                                    Count = 1,
                                    Finish = 1,
                                    RegisteredCount = 1,
                                };
                                proto.PlayerList.Add(new MsgRankListPlayers()
                                {
                                    Rank = info != null ? myRank : 0,
                                    Amount = info != null ? info.TotalPoints : 0,
                                    UID1 = info != null ? info.UID : 0,
                                    UID2 = info != null ? info.UID : 0,
                                    name = info != null ? info.Name : "",
                                    name2 = info != null ? info.Name : "",
                                    Level = info != null ? (byte)info.Level : (byte)0,
                                    Class = info != null ? info.Class : 0,
                                    Mesh = info != null ? info.Mesh : 0,
                                    Member10 = 0,
                                    Member11 = 0,
                                    AvatarFrame = 0
                                });
                                user.Send(stream.CreateRank(proto));
                                break;
                            }
                            #endregion


                            #region MonkRanks
                            if (ranktyp == MsgGenericRanking.RankType.MonkWeeklyRanking)
                            {

                                uint myRank = MonkRanks.GetMyRank(MonkRanks.Type.WeeklyMonk, user.Player.UID);
                                var info = MonkRanks.GetInfo(MonkRanks.Type.WeeklyMonk, user.Player.UID);
                                var proto = new MsgRankProto()
                                {
                                    Mode = (ulong)Action.TotalPoints,
                                    ranktyp = (ulong)ranktyp,
                                    Page = 0,
                                    Count = 1,
                                    Finish = 1,
                                    RegisteredCount = 1,
                                };
                                proto.PlayerList.Add(new MsgRankListPlayers()
                                {
                                    Rank = info != null ? myRank : 0,
                                    Amount = info != null ? info.TotalPoints : 0,
                                    UID1 = info != null ? info.UID : 0,
                                    UID2 = info != null ? info.UID : 0,
                                    name = info != null ? info.Name : "",
                                    name2 = info != null ? info.Name : "",
                                    Level = info != null ? (byte)info.Level : (byte)0,
                                    Class = info != null ? info.Class : 0,
                                    Mesh = info != null ? info.Mesh : 0,
                                    Member10 = 0,
                                    Member11 = 0,
                                    AvatarFrame = 0
                                });
                                user.Send(stream.CreateRank(proto));
                                break;
                            }
                            #endregion

                            #region DuneWanderer
                            if (ranktyp == MsgGenericRanking.RankType.DuneWeekly)
                            {

                                uint myRank = DuneWandererRank.GetMyRank(DuneWandererRank.Type.DuneWandererWeekly, user.Player.UID);
                                var info = DuneWandererRank.GetInfo(DuneWandererRank.Type.DuneWandererWeekly, user.Player.UID);
                                var proto = new MsgRankProto()
                                {
                                    Mode = (ulong)Action.TotalPoints,
                                    ranktyp = (ulong)ranktyp,
                                    Page = 0,
                                    Count = 1,
                                    Finish = 1,
                                    RegisteredCount = 1,
                                };
                                proto.PlayerList.Add(new MsgRankListPlayers()
                                {
                                    Rank = info != null ? myRank : 0,
                                    Amount = info != null ? info.TotalPoints : 0,
                                    UID1 = info != null ? info.UID : 0,
                                    UID2 = info != null ? info.UID : 0,
                                    name = info != null ? info.Name : "",
                                    name2 = info != null ? info.Name : "",
                                    Level = info != null ? (byte)info.Level : (byte)0,
                                    Class = info != null ? info.Class : 0,
                                    Mesh = info != null ? info.Mesh : 0,
                                    Member10 = 0,
                                    Member11 = 0,
                                    AvatarFrame = 0
                                });
                                user.Send(stream.CreateRank(proto));
                                break;
                            }
                            #endregion

                            #region TaoRank
                            if (ranktyp == MsgGenericRanking.RankType.WeeklyTao && !AtributesStatus.IsFire(user.Player.Class))// any viewer; Fire class sees Fire list
                            {
                                uint myRank = WaterRank.GetMyRank(WaterRank.Type.WeeklyTao, user.Player.UID);
                                var info = WaterRank.GetInfo(WaterRank.Type.WeeklyTao, user.Player.UID);
                                var proto = new MsgRankProto()
                                {
                                    Mode = (ulong)Action.TotalPoints,
                                    ranktyp = (ulong)ranktyp,
                                    Page = 0,
                                    Count = 1,
                                    Finish = 1,
                                    RegisteredCount = 1,
                                };
                                proto.PlayerList.Add(new MsgRankListPlayers()
                                {
                                    Rank = info != null ? myRank : 0,
                                    Amount = info != null ? info.TotalPoints : 0,
                                    UID1 = info != null ? info.UID : 0,
                                    UID2 = info != null ? info.UID : 0,
                                    name = info != null ? info.Name : "",
                                    name2 = info != null ? info.Name : "",
                                    Level = info != null ? (byte)info.Level : (byte)0,
                                    Class = info != null ? info.Class : 0,
                                    Mesh = info != null ? info.Mesh : 0,
                                    Member10 = 0,
                                    Member11 = 0,
                                    AvatarFrame = 0
                                });
                                user.Send(stream.CreateRank(proto));
                                break;
                            }
                            if (ranktyp == MsgGenericRanking.RankType.WeeklyTao && AtributesStatus.IsFire(user.Player.Class))
                            {
                                uint myRank = FireRank.GetMyRank(FireRank.Type.WeeklyTao, user.Player.UID);
                                var info = FireRank.GetInfo(FireRank.Type.WeeklyTao, user.Player.UID);
                                var proto = new MsgRankProto()
                                {
                                    Mode = (ulong)Action.TotalPoints,
                                    ranktyp = (ulong)ranktyp,
                                    Page = 0,
                                    Count = 1,
                                    Finish = 1,
                                    RegisteredCount = 1,
                                };
                                proto.PlayerList.Add(new MsgRankListPlayers()
                                {
                                    Rank = info != null ? myRank : 0,
                                    Amount = info != null ? info.TotalPoints : 0,
                                    UID1 = info != null ? info.UID : 0,
                                    UID2 = info != null ? info.UID : 0,
                                    name = info != null ? info.Name : "",
                                    name2 = info != null ? info.Name : "",
                                    Level = info != null ? (byte)info.Level : (byte)0,
                                    Class = info != null ? info.Class : 0,
                                    Mesh = info != null ? info.Mesh : 0,
                                    Member10 = 0,
                                    Member11 = 0,
                                    AvatarFrame = 0
                                });
                                user.Send(stream.CreateRank(proto));
                                break;
                            }
                            #endregion

                            #region Pirate
                            if (ranktyp == MsgGenericRanking.RankType.WeeklyPirate)
                            {
                                uint myRank = PirateRank.GetMyRank(PirateRank.Type.WeeklyPirate, user.Player.UID);
                                var info = PirateRank.GetInfo(PirateRank.Type.WeeklyPirate, user.Player.UID);
                                var proto = new MsgRankProto()
                                {
                                    Mode = (ulong)Action.TotalPoints,
                                    ranktyp = (ulong)ranktyp,
                                    Page = 0,
                                    Count = 1,
                                    Finish = 1,
                                    RegisteredCount = 1,
                                };
                                proto.PlayerList.Add(new MsgRankListPlayers()
                                {
                                    Rank = info != null ? myRank : 0,
                                    Amount = info != null ? info.TotalPoints : 0,
                                    UID1 = info != null ? info.UID : 0,
                                    UID2 = info != null ? info.UID : 0,
                                    name = info != null ? info.Name : "",
                                    name2 = info != null ? info.Name : "",
                                    Level = info != null ? (byte)info.Level : (byte)0,
                                    Class = info != null ? info.Class : 0,
                                    Mesh = info != null ? info.Mesh : 0,
                                    Member10 = 0,
                                    Member11 = 0,
                                    AvatarFrame = 0
                                });
                                user.Send(stream.CreateRank(proto));
                                break;
                            }
                            #endregion

                            #region Dragon
                            if (ranktyp == MsgGenericRanking.RankType.LeeLongWeekly)
                            {

                                uint myRank = LeeLongRank.GetMyRank(LeeLongRank.Type.LeeLongWeekly, user.Player.UID);
                                var info = LeeLongRank.GetInfo(LeeLongRank.Type.LeeLongWeekly, user.Player.UID);
                                var proto = new MsgRankProto()
                                {
                                    Mode = (ulong)Action.TotalPoints,
                                    ranktyp = (ulong)ranktyp,
                                    Page = 0,
                                    Count = 1,
                                    Finish = 1,
                                    RegisteredCount = 1,
                                };
                                proto.PlayerList.Add(new MsgRankListPlayers()
                                {
                                    Rank = info != null ? myRank : 0,
                                    Amount = info != null ? info.TotalPoints : 0,
                                    UID1 = info != null ? info.UID : 0,
                                    UID2 = info != null ? info.UID : 0,
                                    name = info != null ? info.Name : "",
                                    name2 = info != null ? info.Name : "",
                                    Level = info != null ? (byte)info.Level : (byte)0,
                                    Class = info != null ? info.Class : 0,
                                    Mesh = info != null ? info.Mesh : 0,
                                    Member10 = 0,
                                    Member11 = 0,
                                    AvatarFrame = 0
                                });
                                user.Send(stream.CreateRank(proto));
                                break;
                            }
                            #endregion

                            #region ViodragonClubWeekly
                            if (ranktyp == MsgGenericRanking.RankType.ViodragonClubWeekly)
                            {

                                uint myRank = AstredgeRank.GetMyRank(AstredgeRank.Type.AstredgeWeekly, user.Player.UID);
                                var info = AstredgeRank.GetInfo(AstredgeRank.Type.AstredgeWeekly, user.Player.UID);
                                var proto = new MsgRankProto()
                                {
                                    Mode = (ulong)Action.TotalPoints,
                                    ranktyp = (ulong)ranktyp,
                                    Page = 0,
                                    Count = 1,
                                    Finish = 1,
                                    RegisteredCount = 1,
                                };
                                proto.PlayerList.Add(new MsgRankListPlayers()
                                {
                                    Rank = info != null ? myRank : 0,
                                    Amount = info != null ? info.TotalPoints : 0,
                                    UID1 = info != null ? info.UID : 0,
                                    UID2 = info != null ? info.UID : 0,
                                    name = info != null ? info.Name : "",
                                    name2 = info != null ? info.Name : "",
                                    Level = info != null ? (byte)info.Level : (byte)0,
                                    Class = info != null ? info.Class : 0,
                                    Mesh = info != null ? info.Mesh : 0,
                                    Member10 = 0,
                                    Member11 = 0,
                                    AvatarFrame = 0
                                });
                                user.Send(stream.CreateRank(proto));
                                break;
                            }
                            #endregion
                            break;
                        }
                }

            }
            catch (Exception e) { MyConsole.WriteLine(e.ToString()); }
        }
    }
}