using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Qahr.Mobile.Game.MsgServer;
using Qahr.Mobile.Database.DBActions;

namespace Qahr.Mobile.Role.Instance
{
    public class Flowers : IEnumerable<Flowers.Flower>
    {
        public class FlowersRankingToday
        {
            public const int File_Size = 30;
            public Dictionary<uint, uint> Flowers;

            public FlowersRankingToday()
            {
                Flowers = new Dictionary<uint, uint>();
            }
            public void UpdateRank(uint UID, uint Amount)
            {

                if (Flowers.Count < File_Size)
                {
                    Calculate(UID, Amount);
                }
                else
                {
                    var array = Flowers.Values.ToArray();
                    if (array[File_Size -1] <= Amount)
                    {
                        Calculate(UID, Amount);
                    }
                    else
                    {
                        if (Flowers.ContainsKey(UID))
                            Calculate(UID, Amount);
                    }
                }
            }
            public void Calculate(uint UID, uint Amount)
            {
                lock (Flowers)
                {
                    if (!Flowers.ContainsKey(UID))
                        Flowers.Add(UID, Amount);
                    var clients = Flowers.Values.ToArray();
                    var array = Flowers.OrderByDescending(p => p.Value).ToArray();
                    int Rank = 1;
                    Flowers.Clear();
                    foreach (var user_power in array)
                    {
                        if (Rank <= File_Size && Flowers.Count < File_Size)
                        {
                            if (!Flowers.ContainsKey(user_power.Key))
                                Flowers.Add(user_power.Key, user_power.Value);
                        }
                        else
                            break;
                        Rank++;
                    }
                }
            }
        }
        public class FlowerRanking
        {
            public const int File_Size = 100;
            public bool Girls = true;

            public object SynRoot;
            public Dictionary<uint, Flower> RedRoses;
            public Dictionary<uint, Flower> Lilies;
            public Dictionary<uint, Flower> Orchids;
            public Dictionary<uint, Flower> Tulips;
            public Dictionary<uint, Flower> Peonies;
            public FlowerRanking(bool _Girls = true)
            {
                Girls = _Girls;
                SynRoot = new object();
                RedRoses = new Dictionary<uint, Flower>(File_Size);
                Lilies = new Dictionary<uint, Flower>(File_Size);
                Orchids = new Dictionary<uint, Flower>(File_Size);
                Tulips = new Dictionary<uint, Flower>(File_Size);
                Peonies = new Dictionary<uint, Flower>(File_Size);
            }

            public void UpdateRank(Flower AllFlower, Game.MsgServer.MsgFlower.FlowersType Typ)
            {
                lock (SynRoot)
                {
                    switch (Typ)
                    {
                        case Game.MsgServer.MsgFlower.FlowersType.Rouse:
                            CreateRank(RedRoses, AllFlower);
                            break;
                        case Game.MsgServer.MsgFlower.FlowersType.Lilies:
                            CreateRank(Lilies, AllFlower);
                            break;
                        case Game.MsgServer.MsgFlower.FlowersType.Orchids:
                            CreateRank(Orchids, AllFlower);
                            break;
                        case Game.MsgServer.MsgFlower.FlowersType.Tulips:
                            CreateRank(Tulips, AllFlower);
                            break;
                        case Game.MsgServer.MsgFlower.FlowersType.Peonies:
                            CreateRank(Peonies, AllFlower);
                            break;
                    }
                }
            }
            public void CreateRank(Dictionary<uint, Flower> AllRank, Flower AllFlower)
            {
                if (AllRank.Count < File_Size)
                {
                    Calculate(AllRank, AllFlower);
                }
                else
                {
                    var array = AllRank.Values.ToArray();
                    if (array[99].Amount <= AllFlower.Amount)
                    {
                        Calculate(AllRank, AllFlower);
                    }
                    else
                    {
                        if (AllRank.ContainsKey(AllFlower.UID))
                            Calculate(AllRank, AllFlower);
                    }
                }
            }
            public void Calculate(Dictionary<uint, Flower> FlowerRank, Flower Flower)
            {
                lock (FlowerRank)
                {
                    if (!FlowerRank.ContainsKey(Flower.UID))
                        FlowerRank.Add(Flower.UID, Flower);
                    var clients = FlowerRank.Values.ToArray();
                    var array = clients.OrderByDescending(p => p.Amount).ToArray();
                    int Rank = 1;
                    FlowerRank.Clear();
                    foreach (var user_power in array)
                    {
                        int OldRank = user_power.Rank;
                        user_power.Rank = Rank;
                        if (user_power.UID == Flower.UID)
                            Flower.Rank = Rank;
                        if (Rank <= File_Size && FlowerRank.Count < File_Size)
                        {
                            if (!FlowerRank.ContainsKey(user_power.UID))
                                FlowerRank.Add(user_power.UID, user_power);
                        }
                        else
                        {
                            Rank = 0;
                            Flower.Rank = 0;
                            user_power.Rank = 0;
                        }
                        if (OldRank != user_power.Rank)
                        {
                            if (Girls)
                                UpdateRank(Flower.UID, Flower);
                        }
                        Rank++;
                        Client.GameClient user;
                        if (Pool.GamePoll.TryGetValue(Flower.UID, out user))
                        {
                            user.Player.Flowers.ApplayInfo(user.Player);
                        }
                    }
                }
            }
            public unsafe void UpdateRank(uint UID, Flower Flower)
            {
                Client.GameClient user;
                if (Pool.GamePoll.TryGetValue(UID, out user))
                {
                    user.Player.Flowers.UpdateMyRank(user, true);
                }
            }
            
        }

        public class Flower
        {
            public Game.MsgServer.MsgFlower.FlowersType Type;
            public uint Amount;
            public uint Amount2day;

            public int Rank;
            public string Name;
            public uint UID;
            public uint Mesh;
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
            public uint Flag;//??
            public string GuildName = "";
            public uint FrameID;//??
            public uint FlowerFree;//??
            public Flower(Game.MsgServer.MsgFlower.FlowersType typ, uint _uid, string _name)
            {
                UID = _uid;
                Name = _name;
                Type = typ;
            }

            public byte GetGrade()
            {
                switch (Type)
                {
                    case Game.MsgServer.MsgFlower.FlowersType.Rouse: return 1;
                    case Game.MsgServer.MsgFlower.FlowersType.Tulips: return 2;
                    case Game.MsgServer.MsgFlower.FlowersType.Orchids: return 3;
                    case Game.MsgServer.MsgFlower.FlowersType.Lilies: return 4;
                    case Game.MsgServer.MsgFlower.FlowersType.Peonies: return 5;
                }
                return 0;
            }
            public static implicit operator uint(Flower Data)
            {
                return Data.Amount;
            }
            public static Flower operator +(Flower Data, uint amount)
            {
                Data.Amount += amount;
                Data.Amount2day += amount;
                return Data;
            }
        }
        public void ApplayInfo(Role.Player Owner)
        {
            foreach (var info in Owner.Flowers)
            {
                info.Mesh = Owner.Mesh;
                info.HairStyle = Owner.Hair;
                if (Owner.GarmentId == 0)
                    info.Garment = Owner.ArmorId;
                else
                    info.Garment = Owner.GarmentId;
                if (Owner.Owner.Equipment != null)
                {
                    info.Head = Owner.Owner.Equipment.HeadID;
                    info.LeftWeapon = Owner.Owner.Equipment.LeftWeapon;
                    info.RightWeapon = Owner.Owner.Equipment.RightWeapon;
                }
                info.RightWeaponAccessory = Owner.RightWeaponAccessoryId;
                info.LefttWeaponAccessory = Owner.LeftWeaponAccessoryId;
                info.MountArmor = Owner.MountArmorId;
                info.Wing = Owner.WingId;
                if (Owner.SpecialWingID != 0)
                    info.Wing = Owner.SpecialWingID;
                info.WingPlus = Owner.WingPlus;
                info.Title = Owner.SpecialTitleID;
                info.Armor = Owner.ArmorId;
                info.FrameID = Owner.FrameID;
                if (Owner.MyGuild != null)
                    info.GuildName = Owner.MyGuild.GuildName;
            }
        }
        public uint FreeFlowers;
        public int Day;
        public Flower Peonies;
        public Flower RedRoses;
        public Flower Lilies;
        public Flower Orchids;
        public Flower Tulips;

        public uint CreateFlowerIcon(Flower flow, bool Today = false, byte Rank = 0)
        {
            uint ID = (uint)(Today ? 30000001 : 30000002);
            if (flow == null)
                return (uint)(ID + GetRank(Rank));

            if (flow.Rank == 0 || flow.Rank > 100)
                return 0;

            return (uint)(ID + (uint)(100 * (byte)flow.Type) + GetRank(flow.Rank));
        }
        public uint CreateBoyIcon(Flower flow)
        {
            return (uint)(30000402 + (uint)(100 * (byte)flow.Type) + GetRank(flow.Rank));
        }
       
        public ushort GetRank(int rank)
        {
            if (rank == 1)
                return 0;
            if (rank == 2)
                return 10000;
            if (rank == 3)
                return 20000;
            if (rank > 3)
                return 30000;

            return 0;
        }

        public static System.Collections.Concurrent.ConcurrentDictionary<uint, Flowers> ClientPoll = new System.Collections.Concurrent.ConcurrentDictionary<uint, Flowers>();

        public Flowers(uint UID, string Name)
        {
            FreeFlowers = 1;
            RedRoses = new Flower(Game.MsgServer.MsgFlower.FlowersType.Rouse, UID,Name);
            Lilies = new Flower(Game.MsgServer.MsgFlower.FlowersType.Lilies, UID, Name);
            Orchids = new Flower(Game.MsgServer.MsgFlower.FlowersType.Orchids, UID, Name);
            Tulips = new Flower(Game.MsgServer.MsgFlower.FlowersType.Tulips, UID, Name);
            Peonies = new Flower(Game.MsgServer.MsgFlower.FlowersType.Peonies, UID, Name);
        }
        public uint AllFlowersToday()
        {
            return (uint)this.Sum((x) => x.Amount2day);
        }
        public unsafe void UpdateMyRank(Client.GameClient user, bool ReloadScreen = false)
        {
            using (var rec = new ServerSockets.RecycledPacket())
            {

                var stream = rec.GetStream();

                var protos = new MsgRankProto()
                {
                    Mode = (ulong)MsgGenericRanking.Action.InformationRequest,
                    ranktyp = (ulong)MsgGenericRanking.RankType.None,
                    Page = 0,
                    Finish = 1,
                };
                user.Send(stream.CreateRank(protos)); 

                var array = this.Where((f1) => f1.Rank >= 1 && f1.Rank <= 100).ToArray();
                Array.Sort(array, (f1, f2) =>
                {
                    int n_rank = f1.Rank.CompareTo(f2.Rank);
                    if (f2.Rank == f1.Rank)
                        return f2.GetGrade().CompareTo(f1.GetGrade());
                    return n_rank;
                });

                if (array.Length > 0)
                {
                    user.Player.FlowerRank = (int)CreateFlowerIcon(array[0]);
                    foreach (var Flowers in array)
                    {
                        var proto = new MsgRankProto()
                        {
                            Mode = (ulong)MsgGenericRanking.Action.QueryCount,
                            ranktyp = (ulong)(Game.MsgServer.MsgGenericRanking.RankType)user.Player.Flowers.CreateFlowerIcon(Flowers),
                            Page = 0,
                            RegisteredCount = 0,
                            Finish = 1,
                            Count = 1,
                        };
                        proto.PlayerList.Add(new MsgRankListPlayers()
                        {
                            Rank = Flowers != null ? (ulong)Flowers.Rank : (ulong)0,
                            Amount = Flowers != null ? Flowers.Amount : 0,
                            UID1 = Flowers != null ? Flowers.UID : 0,
                            UID2 = Flowers != null ? Flowers.UID : 0,
                            name = Flowers != null ? Flowers.Name : "",
                            name2 = Flowers != null ? Flowers.Name : "",
                            Level = 0,
                            Class = 0,
                            Mesh = Flowers.Mesh,
                            Member10 = 0,
                            Member11 = 0,
                            AvatarFrame = 0,
                        });
                        user.Send(stream.CreateRank(proto));
                    }
                }
                else
                {

                    if (Pool.FlowersRankToday.Flowers.ContainsKey(user.Player.UID))
                    {
                        var RankToday = Pool.FlowersRankToday.Flowers;
                        int Rank = 1;
                        foreach (var FToday in RankToday)
                        {
                            if (FToday.Key == user.Player.UID)
                            {
                                user.Player.FlowerRank = (int)CreateFlowerIcon(null, true, (byte)Rank);
                                var proto = new MsgRankProto()
                                {
                                    Mode = (ulong)MsgGenericRanking.Action.QueryCount,
                                    ranktyp = (ulong)(Game.MsgServer.MsgGenericRanking.RankType)CreateFlowerIcon(null, true, (byte)Rank),
                                    Page = 0,
                                    RegisteredCount = 0,
                                    Finish = 1,
                                };
                                proto.PlayerList.Add(new MsgRankListPlayers()
                                {
                                    Rank = (ulong)Rank,
                                    Amount = FToday.Value,
                                    UID1 = user.Player.UID,
                                    UID2 = user.Player.UID,
                                    name = user.Player.Name,
                                    name2 = user.Player.Name,
                                    Level = user.Player.Level,
                                    Class = user.Player.Class,
                                    Mesh = user.Player.Mesh,
                                    Member10 = 0,
                                    Member11 = 0,
                                    AvatarFrame = 0
                                });
                                user.Send(stream.CreateRank(proto));

                                break;
                            }
                            Rank++;
                        }

                    }
                }
                protos = new MsgRankProto()
                {
                    Mode = (ulong)MsgGenericRanking.Action.InformationRequest,
                    ranktyp = (ulong)MsgGenericRanking.RankType.None,
                    Page = 0,
                    Finish = 1,
                };
                user.Send(stream.CreateRank(protos));
                if (ReloadScreen)
                    user.Player.View.SendView(user.Player.GetArray(stream,false), false);
                user.Player.Flowers.ApplayInfo(user.Player);
                
            }
        }
       
        public IEnumerator<Flower> GetEnumerator()
        {
            yield return RedRoses;
            yield return Lilies;
            yield return Orchids;
            yield return Tulips;
            yield return Peonies;
        }
        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
        {
            return this.GetEnumerator();
        }

        
     
        public override string ToString()
        {
            Database.DBActions.WriteLine writer = new Database.DBActions.WriteLine('/');
            writer.Add(FreeFlowers);
            foreach (var Flow in this)
            {
                writer.Add(Flow.Amount).Add(Flow.Amount2day).Add(Flow.Head)
                .Add(Flow.Mesh)
                .Add(Flow.HairStyle)
                           .Add(Flow.Garment)
                           .Add(Flow.LeftWeapon)
                           .Add(Flow.LefttWeaponAccessory)
                           .Add(Flow.RightWeapon)
                           .Add(Flow.RightWeaponAccessory)
                           .Add(Flow.MountArmor)
                           .Add(Flow.Armor)
                           .Add(Flow.Wing)
                           .Add(Flow.WingPlus)
                           .Add(Flow.Title)
                           .Add(Flow.Flag)
                           .Add(Flow.FrameID)
                       .Add(Flow.GuildName)
                  .Add(Flow.FlowerFree);
               
            }
            return writer.Close();
        }
    }
}