using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections.Concurrent;
using Qahr.Mobile.Game.MsgServer;

namespace Qahr.Mobile.Game.MsgTournaments
{
    public class MsgArena
    {
        public enum ArenaIDs : uint
        {
            ShowPlayerRankList = 0xA,
            QualifierList = 0x6
        }

        public static ConcurrentDictionary<uint, User> ArenaPoll = new ConcurrentDictionary<uint, User>();

        public static User[] Top3 = new User[3];
        public static User[] Top10 = new User[10];
        public static User[] Top1000Today = new User[1000];
        public static User[] Top1000 = new User[1000];

        public List<uint> BlockArenaMaps = new List<uint>() { 11222, 2071, 3071, 12602, 12275, 3030, 10741, 66666, 33333, 22222, 88888,12816, 12546, 12546, 12667,12605,12602,10741,2071,3030,10133,11,7,5,15,14,12,8,20,1730,10134,12099,10133,13140,12344,10070,10504,12344,6521,95152,1770,4551,4552,4556,4557,4560,4561,4564,4564,4566,4568,4569,4570,4571,5661,1486,4447,4448,4449,4450,4451,4452,4453,4454,4455,4456,4457,4458,1484,1485,14850,1518,1508,11241, 1518, 5052, 5061, 5062, 5063, 5064, 5065, 5066, 5053, 5054, 5055, 5056, 5057, 22330, 22331, 22332, 22333, 22334, 22335, 5058, 4057, 5052, 5061, 5062, 5063, 5064, 5065, 5066, 5053, 5054, 5055, 5056, 5057, 5058, 4057, 6011,4020,2351,2352,2353,3820,5040,2057,3825, 9855,6546,12099,8009, 6729, 4003, 5342,4006, 4008, 4009 ,6000, 6001,6521,9988,6891,5599,8521, 1017,1080,1081,2060, 6002, 6003, 601, 700, 1038,MsgClassPKWar.MapID, 1036,22330, 22331, 22332, 22333, 22334, 22335, 22336, 22337, 22338, 1764, Game.MsgTournaments.MsgEliteGroup.WaitingAreaID
        , 1357};

        public ConcurrentDictionary<uint, Client.GameClient> Registered;

        public System.Counter MatchCounter = new System.Counter(1);

        public ConcurrentDictionary<uint, Match> MatchesRegistered;

        public void CreateRankTop10()
        {
            lock (Top10)
            {
                Top10 = new User[10];

                var array = ArenaPoll.Values.ToArray();

                // only players who fought last season (everyone sits in the pool at 4000)
                var Rank = array
                    .Where(p => p.LastSeasonWin + p.LastSeasonLose > 0)
                    .OrderByDescending(p => p.LastSeasonArenaPoints)
                    .ToArray();

                for (int x = 0; x < Rank.Length; x++)
                {
                    if (x == 10)
                        break;

                    var element = Rank[x];
                    Top10[x] = element;
                }
            }
        }

        public void CreateRankTop3()
        {
            lock (Top3)
            {
                Top3 = new User[3];

                var array = ArenaPoll.Values.ToArray();

                // only players who fought last season (everyone sits in the pool at 4000)
                var Rank = array
                    .Where(p => p.LastSeasonWin + p.LastSeasonLose > 0)
                    .OrderByDescending(p => p.LastSeasonArenaPoints)
                    .ToArray();

                for (int x = 0; x < Rank.Length; x++)
                {
                    if (x == 3)
                        break;

                    var element = Rank[x];
                    Top3[x] = element;
                }
            }
        }

        public void CreateRankTop1000()
        {
            lock (Top1000)
            {
                Top1000 = new User[1000];

                var array = ArenaPoll.Values.ToArray();

                // only players who ever fought or earned arena honor
                var Rank = array
                    .Where(p => p.Info.TotalWin + p.Info.TotalLose > 0 || p.Info.HistoryHonor > 0)
                    .OrderByDescending(p => p.Info.CurrentHonor)
                    .ToArray();

                for (int x = 0; x < Rank.Length; x++)
                {
                    if (x == 1000)
                        break;

                    Top1000[x] = Rank[x];
                }
            }
        }

        public class User
        {
            public enum StateType : byte
            {
                None = 0,
                FindMatch = 1,
                WaitForBox = 2,
                WaitForOther = 3,
                Fight = 4
            }

            public enum MatchStatus : byte
            {
                None,
                Winner,
                Loser
            }

            public Game.MsgServer.MsgArenaInfo Info;

            public string Name = "None";
            public uint UID;
            public ushort Level;
            public uint Class;
            public uint Mesh;

            public StateType ArenaState = StateType.None;

            public DateTime AcceptBoxShow = new DateTime();
            public bool AcceptBox = false;

            public MatchStatus QualifierStatus = MatchStatus.None;

            public uint Damage;

            public uint LastSeasonArenaPoints;
            public uint LastSeasonWin;
            public uint LastSeasonLose;
            public uint LastSeasonRank;

            public uint HairStyle;
            public uint Head;
            public uint Garment;
            public uint LeftWeapon;
            public uint LefttWeaponAccessory;
            public uint RightWeapon;
            public uint RightWeaponAccessory;
            public uint MountArmor;
            public uint Armor;
            public uint Wing;
            public uint WingPlus;
            public uint Title;
            public uint Flag;

            public string GuildName = "";

            public uint Cheers;

            public void Reset()
            {
                ArenaState = StateType.None;
                AcceptBox = false;

                Info.Status = MsgServer.MsgArenaInfo.Action.NotSignedUp;

                Cheers = 0;
            }

            public void ApplayInfo(Role.Player player)
            {
                Name = player.Name;
                UID = player.UID;
                Level = player.Level;
                Class = player.Class;
                Mesh = player.Mesh;

                HairStyle = player.Hair;

                if (player.GarmentId == 0)
                    Garment = player.ArmorId;
                else
                    Garment = player.GarmentId;

                if (player.Owner.Equipment != null)
                {
                    Head = player.Owner.Equipment.HeadID;
                    LeftWeapon = player.Owner.Equipment.LeftWeapon;
                    RightWeapon = player.Owner.Equipment.RightWeapon;
                }

                RightWeaponAccessory = player.RightWeaponAccessoryId;
                LefttWeaponAccessory = player.LeftWeaponAccessoryId;

                MountArmor = player.MountArmorId;

                Wing = player.WingId;

                if (player.SpecialWingID != 0)
                    Wing = player.SpecialWingID;

                WingPlus = player.WingPlus;

                Title = player.SpecialTitleID;

                Armor = player.ArmorId;

                if (player.MyGuild != null)
                    GuildName = player.MyGuild.GuildName;

                if (player.ContainFlag(Game.MsgServer.MsgUpdate.Flags.WeeklyPKChampion))
                    Flag = (uint)Game.MsgServer.MsgUpdate.Flags.WeeklyPKChampion;

                else if (player.ContainFlag(Game.MsgServer.MsgUpdate.Flags.TopTrojan))
                    Flag = (uint)Game.MsgServer.MsgUpdate.Flags.TopTrojan;

                else if (player.ContainFlag(Game.MsgServer.MsgUpdate.Flags.TopWarrior))
                    Flag = (uint)Game.MsgServer.MsgUpdate.Flags.TopWarrior;

                else if (player.ContainFlag(Game.MsgServer.MsgUpdate.Flags.TopArcher))
                    Flag = (uint)Game.MsgServer.MsgUpdate.Flags.TopArcher;

                else if (player.ContainFlag(Game.MsgServer.MsgUpdate.Flags.TopNinja))
                    Flag = (uint)Game.MsgServer.MsgUpdate.Flags.TopNinja;

                else if (player.ContainFlag(Game.MsgServer.MsgUpdate.Flags.TopMonk))
                    Flag = (uint)Game.MsgServer.MsgUpdate.Flags.TopMonk;

                else if (player.ContainFlag(Game.MsgServer.MsgUpdate.Flags.TopPirate))
                    Flag = (uint)Game.MsgServer.MsgUpdate.Flags.TopPirate;

                else if (player.ContainFlag(Game.MsgServer.MsgUpdate.Flags.TopDragonLee))
                    Flag = (uint)Game.MsgServer.MsgUpdate.Flags.TopDragonLee;

                else if (player.ContainFlag(Game.MsgServer.MsgUpdate.Flags.TopFireTaoist))
                    Flag = (uint)Game.MsgServer.MsgUpdate.Flags.TopFireTaoist;

                else if (player.ContainFlag(Game.MsgServer.MsgUpdate.Flags.TopWaterTaoist))
                    Flag = (uint)Game.MsgServer.MsgUpdate.Flags.TopWaterTaoist;

                else if (player.ContainFlag(Game.MsgServer.MsgUpdate.Flags.TopSuperGuildWarFiveStars))
                    Flag = (uint)Game.MsgServer.MsgUpdate.Flags.TopSuperGuildWarFiveStars;

                else if (player.ContainFlag(Game.MsgServer.MsgUpdate.Flags.TopSuperGuildWarThreeStars))
                    Flag = (uint)Game.MsgServer.MsgUpdate.Flags.TopSuperGuildWarThreeStars;

                else if (player.ContainFlag(Game.MsgServer.MsgUpdate.Flags.TopSuperGuildWarOneStar))
                    Flag = (uint)Game.MsgServer.MsgUpdate.Flags.TopSuperGuildWarOneStar;

                else if (player.ContainFlag(Game.MsgServer.MsgUpdate.Flags.TopMrsConquer))
                    Flag = (uint)Game.MsgServer.MsgUpdate.Flags.TopMrsConquer;

                else if (player.ContainFlag(Game.MsgServer.MsgUpdate.Flags.TopMrConquer))
                    Flag = (uint)Game.MsgServer.MsgUpdate.Flags.TopMrConquer;

                else if (player.ContainFlag(Game.MsgServer.MsgUpdate.Flags.TopDeputyLeader))
                    Flag = (uint)Game.MsgServer.MsgUpdate.Flags.TopDeputyLeader;

                else if (player.ContainFlag(Game.MsgServer.MsgUpdate.Flags.TopGuildLeader))
                    Flag = (uint)Game.MsgServer.MsgUpdate.Flags.TopGuildLeader;

                else if (player.ContainFlag(Game.MsgServer.MsgUpdate.Flags.TopWindWalker))
                    Flag = (uint)Game.MsgServer.MsgUpdate.Flags.TopWindWalker;

                else if (player.ContainFlag(Game.MsgServer.MsgUpdate.Flags.TopThunderstriker))
                    Flag = (uint)Game.MsgServer.MsgUpdate.Flags.TopThunderstriker;

                else if (player.ContainFlag(Game.MsgServer.MsgUpdate.Flags.TopThunderstriker))
                    Flag = (uint)Game.MsgServer.MsgUpdate.Flags.TopThunderstriker;
            }

            public User()
            {
                Info = new MsgArenaInfo();
            }

            public override string ToString()
            {
                Database.DBActions.WriteLine writer =
                    new Database.DBActions.WriteLine('/');

                writer.Add(UID)
                    .Add(Name)
                    .Add(Level)
                    .Add(Class)
                    .Add(Mesh)

                    .Add(Info.ArenaPoints)
                    .Add(Info.CurrentHonor)
                    .Add(Info.HistoryHonor)
                    .Add(Info.TodayBattles)
                    .Add(Info.TodayWin)
                    .Add(Info.TotalLose)
                    .Add(Info.TotalWin)

                    .Add(LastSeasonArenaPoints)
                    .Add(LastSeasonWin)
                    .Add(LastSeasonLose)
                    .Add(LastSeasonRank)

                    // Appearance
                    .Add(HairStyle)
                    .Add(Head)
                    .Add(Garment)
                    .Add(LeftWeapon)
                    .Add(LefttWeaponAccessory)
                    .Add(RightWeapon)
                    .Add(RightWeaponAccessory)
                    .Add(MountArmor)
                    .Add(Armor)
                    .Add(Wing)
                    .Add(WingPlus)
                    .Add(Title)
                    .Add(Flag)

                    .Add(GuildName);

                return writer.Close();
            }

            internal void Load(string Line)
            {
                if (Line == null)
                    return;

                Database.DBActions.ReadLine reader =
                    new Database.DBActions.ReadLine(Line, '/');

                UID = reader.Read((uint)0);
                Name = reader.Read("None");
                Level = reader.Read((ushort)0);
                Class = reader.Read((uint)0);
                Mesh = reader.Read((uint)0);
                Info.ArenaPoints = reader.Read((uint)0);
                Info.CurrentHonor = reader.Read((uint)0);
                Info.HistoryHonor = reader.Read((uint)0);
                Info.TodayBattles = reader.Read((uint)0);
                Info.TodayWin = reader.Read((uint)0);
                Info.TotalLose = reader.Read((uint)0);
                Info.TotalWin = reader.Read((uint)0);
                LastSeasonArenaPoints = reader.Read((uint)0);
                LastSeasonWin = reader.Read((uint)0);
                LastSeasonLose = reader.Read((uint)0);
                LastSeasonRank = reader.Read((uint)0);
                HairStyle = reader.Read((uint)0);
                Head = reader.Read((uint)0);
                Garment = reader.Read((uint)0);
                LeftWeapon = reader.Read((uint)0);
                LefttWeaponAccessory = reader.Read((uint)0);
                RightWeapon = reader.Read((uint)0);
                RightWeaponAccessory = reader.Read((uint)0);
                MountArmor = reader.Read((uint)0);
                Armor = reader.Read((uint)0);
                Wing = reader.Read((uint)0);
                WingPlus = reader.Read((uint)0);
                Title = reader.Read((uint)0);
                Flag = reader.Read((uint)0);

                GuildName = reader.Read("");
            }
        }

        public ProcesType Proces { get; set; }

        public MsgArena()
        {
            Proces = ProcesType.Dead;

            Registered =
                new ConcurrentDictionary<uint, Client.GameClient>();

            MatchesRegistered =
                new ConcurrentDictionary<uint, Match>();
        }

        public void CheckGroups()
        {
            if (MatchesRegistered.Count > 0)
            {
                DateTime Now = DateTime.Now;

                var ArrayMatches =
                    MatchesRegistered.Values.ToArray();

                foreach (var group in ArrayMatches)
                {
                    if (Now > group.StartTimer.AddSeconds(5))
                    {
                        if (!group.Done)
                        {
                            if (Now > group.StartTimer.AddMinutes(3))
                            {
                                group.End();
                            }
                        }
                        else
                        {
                            if (Now > group.DoneStamp.AddSeconds(4))
                            {
                                group.DoneStamp = DateTime.Now.AddDays(1);

                                group.Export();

                                group.Win(
                                    group.Winner(),
                                    group.Loser());
                            }
                        }
                    }
                }
            }
        }

        public DateTime CreateMatchesStamp = DateTime.Now;

        public void CreateMatches()
        {
            DateTime Timer = DateTime.Now;

            if (Registered.Count < 2)
                return;

            if (Timer > CreateMatchesStamp.AddSeconds(15))
            {
                CreateMatchesStamp = DateTime.Now;

                var array = Registered.Values.ToArray();

                var Players = array
                    .OrderByDescending(
                        p => p.ArenaStatistic.Info.ArenaPoints)
                    .ToArray();

                Client.GameClient user1 = null;
                Client.GameClient user2 = null;

                ConcurrentQueue<Client.GameClient> Remover =
                    new ConcurrentQueue<Client.GameClient>();

                foreach (var user in Players)
                {
                    if (BlockArenaMaps.Contains(user.Player.Map)
                        || user.InQualifier()
                        || user.IsConnectedInterServer())
                    {
                        Remover.Enqueue(user);
                        continue;
                    }

                    if (BlockArenaMaps.Contains(user.Player.Map)
                        || user.InQualifier()
                        || user.Socket.Alive == false
                        || user.IsConnectedInterServer())
                    {
                        Remover.Enqueue(user);
                        continue;
                    }

                    if (user.ArenaStatistic.ArenaState ==
                        User.StateType.FindMatch
                        && user.ArenaStatistic.Info.Status ==
                        MsgServer.MsgArenaInfo.Action.WaitingForOpponent)
                    {
                        if (user1 == null)
                            user1 = user;
                        else if (user2 == null)
                            user2 = user;

                        if (user1 != null && user2 != null)
                            break;
                    }
                }

                if (user1 != null && user2 != null)
                {
                    user1.ArenaStatistic.ArenaState =
                        user2.ArenaStatistic.ArenaState =
                        User.StateType.WaitForBox;

                    user2.ArenaStatistic.AcceptBoxShow =
                        user1.ArenaStatistic.AcceptBoxShow =
                        DateTime.Now.AddSeconds(60);

                    using (var rec =
                        new ServerSockets.RecycledPacket())
                    {
                        var stream = rec.GetStream();

                        user1.Send(
                            stream.ArenaInfoCreate(
                                user1.ArenaStatistic.Info));

                        user2.Send(
                            stream.ArenaInfoCreate(
                                user2.ArenaStatistic.Info));

                        Match match =
                            new Match(
                                user1,
                                user2,
                                MatchCounter.Next);

                        match.SendSignUp(stream, user1);
                        match.SendSignUp(stream, user2);

                        MatchesRegistered.TryAdd(
                            match.MatchUID,
                            match);

                        UnRegistered(user1);
                        UnRegistered(user2);
                    }
                }

                Client.GameClient remover;

                while (Remover.TryDequeue(out remover))
                    Registered.TryRemove(
                        remover.Player.UID,
                        out remover);
            }
        }

        public void VerifyMatches()
        {
            if (MatchesRegistered.Count == 0)
                return;

            var Array =
                MatchesRegistered.Values.ToArray();

            foreach (var match in Array)
            {
                if (match.Players[0] != null &&
                    match.Players[1] != null)
                {
                    if (match.Players[0].Player.Map != 700)
                        if (BlockArenaMaps.Contains(
                            match.Players[0].Player.Map))
                        {
                            match.Win(
                                match.Players[1],
                                match.Players[0]);
                        }

                    if (match.Players[1].Player.Map != 700)
                        if (BlockArenaMaps.Contains(
                            match.Players[1].Player.Map))
                        {
                            match.Win(
                                match.Players[0],
                                match.Players[1]);
                        }

                    if (match.Players[0].ArenaStatistic.ArenaState ==
                        User.StateType.WaitForBox
                        || match.Players[1].ArenaStatistic.ArenaState ==
                        User.StateType.WaitForBox)
                    {
                        if (DateTime.Now >
                            match.Players[0].ArenaStatistic.AcceptBoxShow)
                        {
                            if (match.Players[0].ArenaStatistic.ArenaState ==
                                User.StateType.WaitForBox)
                            {
                                match.Win(
                                    match.Players[1],
                                    match.Players[0]);
                            }
                            else
                            {
                                match.Win(
                                    match.Players[0],
                                    match.Players[1]);
                            }

                            return;
                        }
                    }

                    if (match.Players[0].ArenaStatistic.ArenaState ==
                        User.StateType.WaitForOther
                        && !match.Players[0].ArenaStatistic.AcceptBox)
                    {
                        match.Win(
                            match.Players[1],
                            match.Players[0]);
                    }
                    else if (
                        match.Players[1].ArenaStatistic.ArenaState ==
                        User.StateType.WaitForOther
                        && !match.Players[1].ArenaStatistic.AcceptBox)
                    {
                        match.Win(
                            match.Players[0],
                            match.Players[1]);
                    }
                    else if (
                        match.Players[0].ArenaStatistic.ArenaState ==
                        User.StateType.WaitForOther
                        && match.Players[1].ArenaStatistic.ArenaState ==
                        User.StateType.WaitForOther)
                    {
                        if (!match.Players[0].ArenaStatistic.AcceptBox ||
                            !match.Players[1].ArenaStatistic.AcceptBox)
                        {
                            if (!match.Players[0].ArenaStatistic.AcceptBox)
                            {
                                match.Win(
                                    match.Players[1],
                                    match.Players[0]);
                            }
                            else
                            {
                                match.Win(
                                    match.Players[0],
                                    match.Players[1]);
                            }
                        }
                        else
                        {
                            match.Players[0].ArenaStatistic.ArenaState =
                                match.Players[1].ArenaStatistic.ArenaState =
                                User.StateType.Fight;

                            match.Import();
                        }
                    }
                }
            }
        }

        public static void UpdateRank()
        {
            lock (Top1000Today)
            {
                Top1000Today = new User[1000];

                var array = ArenaPoll.Values.ToArray();

                foreach (var user in array)
                    user.Info.TodayRank = 0;

                // only players who fought today
                var Rank =
                    array.Where(p => p.Info.TodayWin + p.Info.TodayBattles > 0)
                    .OrderByDescending(
                        p => p.Info.ArenaPoints)
                    .ToArray();

                for (int x = 0; x < Rank.Length; x++)
                {
                    if (x == 1000)
                        break;

                    Top1000Today[x] = Rank[x];

                    Rank[x].Info.TodayRank =
                        (uint)(x + 1);
                }
            }
        }

        public unsafe class Match
        {
            public List<uint> Cheerers =
                new List<uint>();

            public ConcurrentDictionary<uint, Client.GameClient> Watchers =
                new ConcurrentDictionary<uint, Client.GameClient>();

            public void BeginWatching(
                ServerSockets.Packet stream,
                Client.GameClient client)
            {
                if (dinamicID == 0)
                {
                    client.SendSysMesage(
                        "The match not started.");

                    return;
                }

                if (!client.Player.Alive)
                {
                    client.SendSysMesage(
                        "Please revive your character to watching that match");

                    return;
                }

                if (client.InQualifier() ||
                    client.IsWatching())
                {
                    client.SendSysMesage(
                        "You're already in a match.");

                    return;
                }

                if (client.Player.ContainFlag(
                    MsgUpdate.Flags.SoulShackle))
                    return;

                if (client.Player.Map == 1005 ||
                    client.Player.Map == 700 ||
                    client.Player.Map == 5050 ||
                    client.Player.Map == 22334 ||
                    client.Player.Map == 22333 ||
                    client.Player.Map == 22332 ||
                    client.Player.Map == 22331 ||
                    client.Player.Map == 4057 ||
                    client.Player.Map == 22335 ||
                    client.Player.Map == 5052 ||
                    client.Player.Map == 5053 ||
                    client.Player.Map == 5054 ||
                    client.Player.Map == 5055 ||
                    client.Player.Map == 5056 ||
                    client.Player.Map == 5057 ||
                    client.Player.Map == 5058 ||
                    client.Player.Map == 5059 ||
                    client.Player.Map == 5060 ||
                    client.Player.Map == 5061 ||
                    client.Player.Map == 5062 ||
                    client.Player.Map == 5063 ||
                    client.Player.Map == 5064 ||
                    client.Player.Map == 5065 ||
                    client.Player.Map == 5066 ||
                    client.Player.Map == 5067 ||
                    client.Player.Map == 5068 ||
                    client.Player.Map == 5069 ||
                    client.Player.Map == 5070 ||
                    client.Player.Map == 22330 ||
                    client.Player.Map == 1487 ||
                    client.Player.Map == 5661 ||
                    client.Player.Map == 1486 ||
                    client.Player.Map == 1483 ||
                    client.Player.Map == 1484 ||
                    client.Player.Map == 1485 ||
                    client.Player.Map == 6526 ||
                    client.Player.Map == 6522 ||
                    client.Player.Map == 22341 ||
                    client.Player.Map == 22340 ||
                    client.Player.Map == 26400 ||
                    client.Player.Map == 2515 ||
                    client.Player.Map == 6521 ||
                    client.Player.Map == 12816 ||
                    client.Player.Map == 12099 ||
                    client.Player.Map == 6570 ||
                    client.Player.Map == 3581 ||
                    client.Player.Map == 50017 ||
                    client.Player.Map == 50105 ||
                    client.Player.Map == 50104 ||
                    client.Player.Map == 50021 ||
                    client.Player.Map == 50020 ||
                    client.Player.Map == 50019 ||
                    client.Player.Map == 50018 ||
                    client.Player.Map == 50103 ||
                    client.Player.Map == 50102 ||
                    client.Player.Map == 50101 ||
                    client.Player.Map == 50100 ||
                    client.Player.Map == 50016 ||
                    client.Player.Map == 1508 ||
                    client.Player.Map == 1518)
                {
                    client.SendSysMesage("#10 .");
                    return;
                }

                if (!Watchers.ContainsKey(
                    client.Player.UID))
                {
                    client.ArenaWatchingGroup = this;

                    client.Teleport(
                        (ushort)Pool.GetRandom.Next(35, 70),
                        (ushort)Pool.GetRandom.Next(35, 70),
                        700,
                        dinamicID);

                    if (Watchers.TryAdd(
                        client.Player.UID,
                        client))
                    {
                        stream.ArenaWatchersCreate(
                            MsgArenaWatchers.WatcherTyp.RequestView,
                            0,
                            0,
                            0,
                            Players[0].ArenaStatistic.Cheers,
                            Players[1].ArenaStatistic.Cheers);

                        client.Send(
                            stream.ArenaWatchersFinalize());

                        stream.ArenaWatchersCreate(
                            MsgArenaWatchers.WatcherTyp.Watchers,
                            0,
                            0,
                            (uint)Watchers.Count,
                            Players[0].ArenaStatistic.Cheers,
                            Players[1].ArenaStatistic.Cheers);

                        var array =
                            Watchers.Values.ToArray();

                        for (int x = 0;
                             x < Watchers.Count;
                             x++)
                            stream.AddItemArenaWatchers(
                                array[x].ArenaStatistic);

                        stream.ArenaWatchersFinalize();

                        foreach (var user in Watchers.Values)
                            user.Send(stream);

                        foreach (var user in Players)
                            user.Send(stream);

                        SendScore();
                    }
                }
            }

            public unsafe void DoLeaveWatching(
                Client.GameClient client)
            {
                Client.GameClient remover;

                if (client.IsWatching() &&
                    Watchers.TryRemove(
                        client.Player.UID,
                        out remover) &&
                    client.Player.Map == 700 &&
                    client.Player.DynamicID == dinamicID)
                {
                    using (var rec =
                        new ServerSockets.RecycledPacket())
                    {
                        var stream = rec.GetStream();

                        stream.ArenaWatchersCreate(
                            MsgArenaWatchers.WatcherTyp.Leave,
                            0,
                            0,
                            (uint)Watchers.Count,
                            Players[0].ArenaStatistic.Cheers,
                            Players[1].ArenaStatistic.Cheers);

                        var array =
                            Watchers.Values.ToArray();

                        for (int x = 0;
                             x < Watchers.Count;
                             x++)
                            stream.AddItemArenaWatchers(
                                array[x].ArenaStatistic);

                        client.Send(
                            stream.ArenaWatchersFinalize());

                        stream.ArenaWatchersCreate(
                            MsgArenaWatchers.WatcherTyp.Watchers,
                            0,
                            0,
                            (uint)Watchers.Count,
                            Players[0].ArenaStatistic.Cheers,
                            Players[1].ArenaStatistic.Cheers);

                        for (int x = 0;
                             x < Watchers.Count;
                             x++)
                            stream.AddItemArenaWatchers(
                                array[x].ArenaStatistic);

                        stream.ArenaWatchersFinalize();

                        foreach (var user in Watchers.Values)
                            user.Send(stream);

                        foreach (var user in Players)
                            user.Send(stream);

                        stream.ArenaWatchersCreate(
                            MsgArenaWatchers.WatcherTyp.Leave,
                            0,
                            0,
                            0,
                            0,
                            0);

                        client.Send(
                            stream.ArenaWatchersFinalize());
                    }

                    SendScore();

                    client.ArenaWatchingGroup = null;

                    client.TeleportCallBack();
                }

                client.ArenaWatchingGroup = null;
            }

            public unsafe void DoCheer(
                ServerSockets.Packet stream,
                Client.GameClient client,
                uint uid)
            {
                if (client.IsWatching() &&
                    !Cheerers.Contains(client.Player.UID))
                {
                    Cheerers.Add(client.Player.UID);

                    if (Players[0].Player.UID == uid)
                    {
                        Players[0].ArenaStatistic.Cheers++;
                    }
                    else if (Players[1].ArenaStatistic.UID == uid)
                    {
                        Players[1].ArenaStatistic.Cheers++;
                    }

                    stream.ArenaWatchersCreate(
                        MsgArenaWatchers.WatcherTyp.Watchers,
                        0,
                        0,
                        (uint)Watchers.Count,
                        Players[0].ArenaStatistic.Cheers,
                        Players[1].ArenaStatistic.Cheers);

                    var array =
                        Watchers.Values.ToArray();

                    for (int x = 0;
                         x < Watchers.Count;
                         x++)
                        stream.AddItemArenaWatchers(
                            array[x].ArenaStatistic);

                    stream.ArenaWatchersFinalize();

                    foreach (var user in Watchers.Values)
                        user.Send(stream);

                    foreach (var user in Players)
                        user.Send(stream);

                    SendScore();
                }
            }

            public bool Done;
            public bool Imported = false;
            public uint dinamicID;

            private uint UID;

            public DateTime DoneStamp;
            public DateTime StartTimer;

            public uint MatchUID
            {
                get { return UID; }
            }

            public Client.GameClient Winner()
            {
                var client =
                    Players.Where(
                        p => p.ArenaStatistic.QualifierStatus !=
                        User.MatchStatus.Loser
                        &&
                        p.ArenaStatistic.QualifierStatus !=
                        User.MatchStatus.None)
                    .SingleOrDefault();

                if (client == null)
                    return Players[0];

                return client;
            }

            public Client.GameClient Loser()
            {
                var client =
                    Players.Where(
                        p => p.ArenaStatistic.QualifierStatus ==
                        User.MatchStatus.Loser)
                    .SingleOrDefault();

                if (client == null)
                    return Players[0];

                return client;
            }

            public Client.GameClient[] Players;

            public Match(
                Client.GameClient user1,
                Client.GameClient user2,
                uint _uid)
            {
                Players = new Client.GameClient[2];

                Players[0] = user1;
                Players[1] = user2;

                UID = _uid;

                DoneStamp = new DateTime();

                user1.ArenaMatch =
                    user2.ArenaMatch =
                    this;

                user1.ArenaStatistic.QualifierStatus =
                    user2.ArenaStatistic.QualifierStatus =
                    User.MatchStatus.None;

                StartTimer = DateTime.Now;
            }

            public bool TryGetOpponent(
                uint MyUID,
                out Client.GameClient client)
            {
                foreach (var user in Players)
                {
                    if (MyUID != user.Player.UID)
                    {
                        client = user;
                        return true;
                    }
                }

                client = null;

                return false;
            }

            public void SendSignUp(
                ServerSockets.Packet stream,
                Client.GameClient user)
            {
                user.Send(
                    stream.ArenaSignupCreate(
                        MsgArenaSignup.DialogType.StartCountDown,
                        MsgArenaSignup.DialogButton.SignUp,
                        user));
            }

            public void Export()
            {
                if (Imported)
                {
                    Match m_math;

                    MsgSchedules.Arena.MatchesRegistered.TryRemove(
                        UID,
                        out m_math);

                    foreach (var user in Watchers.Values)
                        DoLeaveWatching(user);

                    foreach (var user in Players)
                    {
                        user.Player.RemoveFlag(
                            MsgUpdate.Flags.Poisoned);

                        user.TeleportCallBack();

                        user.Player.RestorePkMode();
                    }
                }
            }

            public void Win(
                Client.GameClient winner,
                Client.GameClient loser)
            {
                winner.ArenaStatistic.QualifierStatus =
                    User.MatchStatus.Winner;

                loser.ArenaStatistic.QualifierStatus =
                    User.MatchStatus.Loser;

                if (winner.ArenaMatch != null &&
                    loser.ArenaMatch != null)
                {
                    using (var rec =
                        new ServerSockets.RecycledPacket())
                    {
                        var stream = rec.GetStream();

                        winner.ArenaMatch = null;
                        loser.ArenaMatch = null;

                        winner.ArenaStatistic.Info.Status =
                            MsgServer.MsgArenaInfo.Action.NotSignedUp;

                        winner.Send(
                            stream.ArenaInfoCreate(
                                winner.ArenaStatistic.Info));

                        winner.ArenaStatistic.ArenaState =
                            User.StateType.FindMatch;

                        loser.ArenaStatistic.Info.Status =
                            MsgServer.MsgArenaInfo.Action.NotSignedUp;

                        loser.Send(
                            stream.ArenaInfoCreate(
                                loser.ArenaStatistic.Info));

                        loser.ArenaStatistic.ArenaState =
                            User.StateType.FindMatch;

                        winner.ArenaPoints += (uint)40;

                        if (winner.ArenaStatistic.Info.TodayWin == 9)
                        {
                            winner.Player.AddChampionPoints(200);

                            winner.SendSysMesage(
                                "You received 200 ChampionPoints.",
                                MsgMessage.ChatMode.System);

                            if (winner.Inventory.HaveSpace(1))
                            {
                                winner.Inventory.AddItemWitchStack(
                                    723912,
                                    0,
                                    1,
                                    stream);
                            }
                            else
                            {
                                winner.Inventory.AddReturnedItem(
                                    stream,
                                    723912);
                            }
                        }

                        if (winner.ArenaStatistic.Info.TodayWin == 20)
                        {
                            winner.HeroRewards.AddGoal(506);
                        }

                        winner.ArenaStatistic.Info.TodayWin++;
                        winner.ArenaStatistic.Info.TotalWin++;

                        loser.ArenaStatistic.Info.TodayBattles++;
                        loser.ArenaStatistic.Info.TotalLose++;
                        global::Qahr.Mobile.Phone.MobileDaily.Event(loser, "arena");// QahrMobile: mobile daily Qualifier (the winner counts through Activeness task 7)

                        winner.Activeness.IncreaseTask(7);
                        winner.Activeness.IncreaseTask(19);
                        winner.Activeness.IncreaseTask(31);
                        winner.Activeness.IncreaseTask(36);

                        // 20 matches today (TodayBattles counts losses only): fires once, not on every later win
                        if (winner.ArenaStatistic.Info.TodayWin + winner.ArenaStatistic.Info.TodayBattles == 20)
                        {
                            winner.Player.AddChampionPoints(200);

                            winner.SendSysMesage(
                                "You received 200 ChampionPoints.",
                                MsgMessage.ChatMode.System);

                            if (winner.Inventory.HaveSpace(1))
                            {
                                winner.Inventory.AddItemWitchStack(
                                    723912,
                                    0,
                                    1,
                                    stream);
                            }
                            else
                            {
                                winner.Inventory.AddReturnedItem(
                                    stream,
                                    723912);
                            }
                        }

                        if (loser.ArenaStatistic.Info.TodayBattles == 20)
                        {
                            loser.Player.AddChampionPoints(200);

                            loser.SendSysMesage(
                                "You received 200 ChampionPoints.",
                                MsgMessage.ChatMode.System);

                            if (loser.Inventory.HaveSpace(1))
                            {
                                loser.Inventory.AddItemWitchStack(
                                    723912,
                                    0,
                                    1,
                                    stream);
                            }
                            else
                            {
                                loser.Inventory.AddReturnedItem(
                                    stream,
                                    723912);
                            }
                        }

                        UpdateRank();

                        Game.MsgTournaments.MsgSchedules.Arena.CreateRankTop3();
                        Game.MsgTournaments.MsgSchedules.Arena.CreateRankTop10();

                        StringBuilder builder =
                            new StringBuilder();

                        if (winner.Player.MyGuild != null)
                        {
                            builder.Append("(");
                            builder.Append(
                                winner.Player.MyGuild.GuildName.ToString());
                            builder.Append(") ");
                        }

                        builder.Append(winner.Player.Name);
                        builder.Append(" has defeated ");

                        if (loser.Player.MyGuild != null)
                        {
                            builder.Append("(");
                            builder.Append(
                                loser.Player.MyGuild.GuildName.ToString());
                            builder.Append(") ");
                        }

                        builder.Append(loser.Player.Name);

                        if (winner.ArenaStatistic.Info.TodayRank > 0)
                        {
                            builder.Append(
                                " in the Qualifier, and is currently ranked No. ");

                            builder.Append(
                                winner.ArenaStatistic.Info.TodayRank);
                        }
                        else
                        {
                            builder.Append(
                                " in the Qualifier");
                        }

                        builder.Append(".");

                        Server.SendGlobalPacket(
                            new Game.MsgServer.MsgMessage(
                                builder.ToString(),
                                MsgServer.MsgMessage.MsgColor.red,
                                MsgServer.MsgMessage.ChatMode.Qualifier)
                            .GetArray(stream));

                        loser.Send(
                            stream.ArenaSignupCreate(
                                MsgArenaSignup.DialogType.Dialog2,
                                MsgArenaSignup.DialogButton.SignUp,
                                loser));

                        winner.Send(
                            stream.ArenaSignupCreate(
                                MsgArenaSignup.DialogType.Dialog2,
                                MsgArenaSignup.DialogButton.Win,
                                winner));

                        winner.ArenaStatistic.Reset();
                        loser.ArenaStatistic.Reset();

                        winner.Send(
                            stream.ArenaInfoCreate(
                                winner.ArenaStatistic.Info));

                        loser.Send(
                            stream.ArenaInfoCreate(
                                loser.ArenaStatistic.Info));

                        Match m_math;

                        MsgSchedules.Arena.MatchesRegistered.TryRemove(
                            UID,
                            out m_math);
                    }
                }
            }

            public void End()
            {
                End(
                    (Players[0].ArenaStatistic.Damage >
                     Players[1].ArenaStatistic.Damage)
                    ? Players[1]
                    : Players[0]);
            }

            public void End(Client.GameClient loser)
            {
                if (Done)
                    return;

                Done = true;

                Players[0].Player.ProtectAttack(5 * 1000);
                Players[1].Player.ProtectAttack(5 * 1000);

                if (Players[0].Player.UID ==
                    loser.Player.UID)
                {
                    Players[0].ArenaStatistic.QualifierStatus =
                        User.MatchStatus.Loser;

                    Players[1].ArenaStatistic.QualifierStatus =
                        User.MatchStatus.Winner;
                }
                else
                {
                    Players[1].ArenaStatistic.QualifierStatus =
                        User.MatchStatus.Loser;

                    Players[0].ArenaStatistic.QualifierStatus =
                        User.MatchStatus.Winner;
                }

                using (var rec =
                    new ServerSockets.RecycledPacket())
                {
                    var stream = rec.GetStream();

                    loser.Send(
                        stream.ArenaSignupCreate(
                            MsgArenaSignup.DialogType.Dialog,
                            MsgArenaSignup.DialogButton.Lose,
                            loser));

                    Winner().Send(
                        stream.ArenaSignupCreate(
                            MsgArenaSignup.DialogType.Dialog,
                            MsgArenaSignup.DialogButton.Win,
                            Winner()));
                }

                Winner().HeroRewards.AddGoal(204);

                DoneStamp = DateTime.Now;
            }

            public void Import()
            {
                if (!Imported)
                {
                    var map = Pool.ServerMaps[700];

                    dinamicID =
                        map.GenerateDynamicID();

                    foreach (var user in Players)
                    {
                        user.ArenaStatistic.Damage = 0;

                        ushort x = 0;
                        ushort y = 0;

                        map.GetRandCoord(
                            ref x,
                            ref y);

                        user.Teleport(
                            x,
                            y,
                            700,
                            dinamicID);

                        user.Player.ProtectJumpAttack(10);

                        if (user.Player.MyJiangHu != null)
                        {
                            if (user.Player.JiangHuActive != 0)
                            {
                                user.Player.PkMode =
                                    Role.Flags.PKMode.Capture;

                                user.Player.MyJiangHu.OnJiangMode =
                                    false;

                                user.Player.MyJiangHu.DisableJiang(
                                    user);
                            }
                        }

                        Client.GameClient Opponent;

                        if (TryGetOpponent(
                            user.Player.UID,
                            out Opponent))
                        {
                            using (var rec =
                                new ServerSockets.RecycledPacket())
                            {
                                var stream = rec.GetStream();

                                Opponent.Send(
                                    stream.ArenaSignupCreate(
                                        MsgArenaSignup.DialogType.StartTheFight,
                                        MsgArenaSignup.DialogButton.SignUp,
                                        user));

                                Opponent.Send(
                                    stream.ArenaSignupCreate(
                                        MsgArenaSignup.DialogType.Match,
                                        MsgArenaSignup.DialogButton.MatchOn,
                                        user));
                            }
                        }

                        user.Player.SetPkMode(
                            Role.Flags.PKMode.PK);
                    }

                    Imported = true;

                    SendScore();
                }
            }

            public void SendScore()
            {
                using (var rec =
                    new ServerSockets.RecycledPacket())
                {
                    var stream = rec.GetStream();

                    stream =
                        stream.ArenaMatchScoreCreate(
                            Players[0].Player.UID,
                            Players[0].ArenaStatistic.Name,
                            Players[0].ArenaStatistic.Damage,

                            Players[1].Player.UID,
                            Players[1].ArenaStatistic.Name,
                            Players[1].ArenaStatistic.Damage);

                    foreach (var user in Players)
                        user.Send(stream);

                    foreach (var user in Watchers.Values)
                        user.Send(stream);
                }
            }
        }

        public void DoSignup(
            ServerSockets.Packet stream,
            Client.GameClient client)
        {
            if (client.ArenaStatistic.Info.Status !=
                MsgServer.MsgArenaInfo.Action.NotSignedUp)
            {
                client.SendSysMesage(
                    "You already joined a qualifier arena! Quit the other one and sign up again.",
                    MsgServer.MsgMessage.ChatMode.Agate,
                    MsgServer.MsgMessage.MsgColor.red);

                return;
            }

            if (client.InQualifier())
                return;

            if (client.ArenaStatistic.Info.ArenaPoints == 0)
                return;

            if (BlockArenaMaps.Contains(client.Player.Map))
                return;

            if (!Game.MsgTournaments.MsgArena.ArenaPoll.ContainsKey(
                client.Player.UID))
            {
                Game.MsgTournaments.MsgArena.ArenaPoll.TryAdd(
                    client.Player.UID,
                    client.ArenaStatistic);
            }

            Registered.TryAdd(
                client.Player.UID,
                client);

            client.ArenaStatistic.Info.Status =
                MsgServer.MsgArenaInfo.Action.WaitingForOpponent;

            client.ArenaStatistic.ArenaState =
                User.StateType.FindMatch;

            client.Send(
                stream.ArenaInfoCreate(
                    client.ArenaStatistic.Info));
        }

        public void UnRegistered(
            Client.GameClient client)
        {
            Client.GameClient remover;

            Registered.TryRemove(
                0,
                out remover);

            Registered.TryRemove(
                client.Player.UID,
                out remover);
        }

        public void DoQuit(
            ServerSockets.Packet stream,
            Client.GameClient client,
            bool InMathat = false)
        {
            if (client.ArenaMatch != null)
                client.ArenaMatch.End(client);
            else
                client.ArenaStatistic.Reset();

            UnRegistered(client);

            client.ArenaStatistic.Info.Status =
                MsgServer.MsgArenaInfo.Action.NotSignedUp;

            client.Send(
                stream.ArenaInfoCreate(
                    client.ArenaStatistic.Info));
        }

        public unsafe void DoGiveUp(
            ServerSockets.Packet stream,
            Client.GameClient client)
        {
            if (client.ArenaStatistic.ArenaState ==
                User.StateType.WaitForBox)
            {
                client.ArenaStatistic.AcceptBox = false;

                client.ArenaStatistic.ArenaState =
                    User.StateType.WaitForOther;
            }
            else
            {
                client.ArenaStatistic.Info.Status =
                    MsgServer.MsgArenaInfo.Action.WaitingInactive;

                client.Send(
                    stream.ArenaInfoCreate(
                        client.ArenaStatistic.Info));

                if (client.ArenaMatch != null)
                {
                    Client.GameClient Opponent;

                    if (client.ArenaMatch.TryGetOpponent(
                        client.Player.UID,
                        out Opponent))
                    {
                        Opponent.Send(
                            stream.ArenaSignupCreate(
                                MsgArenaSignup.DialogType.OpponentGaveUp,
                                MsgArenaSignup.DialogButton.SignUp,
                                client));

                        Opponent.ArenaStatistic.Info.Status =
                            MsgServer.MsgArenaInfo.Action.NotSignedUp;

                        client.ArenaStatistic.Info.Status =
                            MsgServer.MsgArenaInfo.Action.NotSignedUp;

                        Opponent.Send(
                            stream.ArenaInfoCreate(
                                Opponent.ArenaStatistic.Info));

                        client.Send(
                            stream.ArenaInfoCreate(
                                client.ArenaStatistic.Info));
                    }

                    if (!client.ArenaMatch.Done)
                    {
                        client.ArenaMatch.End(client);
                    }
                    else
                    {
                        if (Opponent != null)
                            client.ArenaMatch.Win(
                                Opponent,
                                client);
                        else
                            client.ArenaMatch.End(client);
                    }
                }
            }
        }
    }
}
