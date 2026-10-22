using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections.Concurrent;
using Qahr.Mobile.Game.MsgServer;
using System.IO;

namespace Qahr.Mobile.Role.Instance
{

    public class ChiRank
    {
        public const int File_Size = 50;

        public Dictionary<uint, Chi.ChiPower> Dragon;
        public Dictionary<uint, Chi.ChiPower> Phoenix;
        public Dictionary<uint, Chi.ChiPower> Turtle;
        public Dictionary<uint, Chi.ChiPower> Tiger;

        public ChiRank()
        {
            Dragon = new Dictionary<uint, Chi.ChiPower>(File_Size);
            Phoenix = new Dictionary<uint, Chi.ChiPower>(File_Size);
            Turtle = new Dictionary<uint, Chi.ChiPower>(File_Size);
            Tiger = new Dictionary<uint, Chi.ChiPower>(File_Size);
        }
        public void Upadte(Dictionary<uint, Chi.ChiPower> power, Chi.ChiPower MyPower)
        {
            if (power.Count < File_Size)
            {
                Calculate(power, MyPower);
            }
            else
            {
                var array = power.Values.ToArray();
                if (array[49].Score <= MyPower.Score)
                {
                    Calculate(power, MyPower);
                }
                else
                {
                    if (power.ContainsKey(MyPower.UID))
                        Calculate(power, MyPower);
                }
            }
        }
        public void Calculate(Dictionary<uint, Chi.ChiPower> power, Chi.ChiPower MyPower)
        {
            lock (power)
            {
                if (power.ContainsKey(MyPower.UID))
                    power.Remove(MyPower.UID);
                power.Add(MyPower.UID, MyPower);
                var clients = power.Values.ToArray();
                var array = clients.OrderByDescending(p => p.Score).ToArray();
                int Rank = 1;
                power.Clear();
                foreach (var user_power in array)
                {
                    int OldRank = user_power.Rank;
                    user_power.Rank = Rank;
                    if (user_power.UID == MyPower.UID)
                        MyPower.Rank = Rank;
                    if (Rank <= File_Size)
                    {
                        if (!power.ContainsKey(user_power.UID))
                            power.Add(user_power.UID, user_power);
                    }
                    if (OldRank != user_power.Rank)
                    {
                        SendUpdate(user_power.UID, user_power);
                    }
                    Rank++;
                }
            }
        }
        public unsafe void SendUpdate(uint UID, Chi.ChiPower MyPower)
        {
            Client.GameClient user;
            if (Pool.GamePoll.TryGetValue(UID, out user))
            {
                using (var rec = new ServerSockets.RecycledPacket())
                {
                    var stream = rec.GetStream();
                    var Info = new MsgRankProto()
                    {
                        Mode = (ulong)MsgGenericRanking.Action.QueryCount,
                        ranktyp = (ulong)(Game.MsgServer.MsgGenericRanking.RankType)(60000000 + (byte)MyPower.Type),
                        Page = 0,
                        RegisteredCount = 0,
                        Finish = 1,
                    };
                    Info.PlayerList.Add(new MsgRankListPlayers()
                    {
                        Rank = (ulong)(MyPower.Rank),
                        Amount = (uint)MyPower.Score,
                        UID1 = MyPower.UID,
                        UID2 = MyPower.UID,
                        name = MyPower.Name,
                        name2 = MyPower.Name,
                        Level = 0,
                        Class = 0,
                        Mesh = 0,
                        Member10 = 0,
                        Member11 = 0,
                        AvatarFrame = 0
                    });
                    user.Send(stream.CreateRank(Info));
                }
                user.Equipment.QueryEquipment(user.Equipment.Alternante, false);
            }
        }
    }
    public class Chi : IEnumerable<Chi.ChiPower>
    {
        public void Login(Client.GameClient client)
        {
            List<MsgChiInfo.ChiPowerType> Removed = new List<MsgChiInfo.ChiPowerType>(4);
            if (client.Player.MyChi.DragonTime != 0)
            {
                if (DateTime.Now > DateTime.FromBinary(client.Player.MyChi.DragonTime))
                {
                    Removed.Add(MsgChiInfo.ChiPowerType.Dragon);
                }
            }
            if (client.Player.MyChi.PhoenixTime != 0)
            {
                if (DateTime.Now > DateTime.FromBinary(client.Player.MyChi.PhoenixTime))
                {
                    Removed.Add(MsgChiInfo.ChiPowerType.Phoenix);
                }
            }
            if (client.Player.MyChi.TigerTime != 0)
            {
                if (DateTime.Now > DateTime.FromBinary(client.Player.MyChi.TigerTime))
                {
                    Removed.Add(MsgChiInfo.ChiPowerType.Tiger);
                }
            }
            if (client.Player.MyChi.TurtleTime != 0)
            {
                if (DateTime.Now > DateTime.FromBinary(client.Player.MyChi.TurtleTime))
                {
                    Removed.Add(MsgChiInfo.ChiPowerType.Turtle);
                }
            }
            if (Removed.Count > 0)
            {
                using (var recycle = new ServerSockets.RecycledPacket())
                {
                    var stream = recycle.GetStream();
                    client.Send(stream.CreateExpireNotify(Removed));
                }
            }
        }
        public static int ChisuperValues(Game.MsgServer.MsgChiInfo.ChiAttribute attribute)
        {
            switch (attribute)
            {
                case Game.MsgServer.MsgChiInfo.ChiAttribute.CriticalStrike: return 181;
                case Game.MsgServer.MsgChiInfo.ChiAttribute.SkillCriticalStrike: return 181;
                case Game.MsgServer.MsgChiInfo.ChiAttribute.Immunity: return 181;
                case Game.MsgServer.MsgChiInfo.ChiAttribute.Breakthrough: return 181;
                case Game.MsgServer.MsgChiInfo.ChiAttribute.Counteraction: return 181;
                case Game.MsgServer.MsgChiInfo.ChiAttribute.HPAdd: return 3274;
                case Game.MsgServer.MsgChiInfo.ChiAttribute.AddAttack: return 1864;
                case Game.MsgServer.MsgChiInfo.ChiAttribute.AddMagicAttack: return 2319;
                case Game.MsgServer.MsgChiInfo.ChiAttribute.AddMagicDefense: return 231;
                case Game.MsgServer.MsgChiInfo.ChiAttribute.PhysicalDamageIncrease: return 459;
                case Game.MsgServer.MsgChiInfo.ChiAttribute.PhysicalDamageDecrease: return 277;
                case Game.MsgServer.MsgChiInfo.ChiAttribute.MagicDamageIncrease: return 277;
                case Game.MsgServer.MsgChiInfo.ChiAttribute.MagicDamageDecrease: return 277;
                default: return 100;
            }
        }
        public static int ChiMaxValues(Game.MsgServer.MsgChiInfo.ChiAttribute attribute)
        {
            switch (attribute)
            {
                case Game.MsgServer.MsgChiInfo.ChiAttribute.CriticalStrike: return 200;
                case Game.MsgServer.MsgChiInfo.ChiAttribute.SkillCriticalStrike: return 200;
                case Game.MsgServer.MsgChiInfo.ChiAttribute.Immunity: return 200;
                case Game.MsgServer.MsgChiInfo.ChiAttribute.Breakthrough: return 200;
                case Game.MsgServer.MsgChiInfo.ChiAttribute.Counteraction: return 200;
                case Game.MsgServer.MsgChiInfo.ChiAttribute.HPAdd: return 3500;
                case Game.MsgServer.MsgChiInfo.ChiAttribute.AddAttack: return 2000;
                case Game.MsgServer.MsgChiInfo.ChiAttribute.AddMagicAttack: return 2500;
                case Game.MsgServer.MsgChiInfo.ChiAttribute.AddMagicDefense: return 250;
                case Game.MsgServer.MsgChiInfo.ChiAttribute.PhysicalDamageIncrease: return 500;
                case Game.MsgServer.MsgChiInfo.ChiAttribute.PhysicalDamageDecrease: return 500;
                case Game.MsgServer.MsgChiInfo.ChiAttribute.MagicDamageIncrease: return 300;
                case Game.MsgServer.MsgChiInfo.ChiAttribute.MagicDamageDecrease: return 300;
                default: return 100;
            }
        }
        public static int ChiSuper(Game.MsgServer.MsgChiInfo.ChiAttribute power)
        {
            switch (power)
            {
                case Game.MsgServer.MsgChiInfo.ChiAttribute.CriticalStrike:
                    return 182;
                case Game.MsgServer.MsgChiInfo.ChiAttribute.SkillCriticalStrike:
                    return 182;
                case Game.MsgServer.MsgChiInfo.ChiAttribute.Immunity:
                    return 182;
                case Game.MsgServer.MsgChiInfo.ChiAttribute.Breakthrough:
                    return 182;
                case Game.MsgServer.MsgChiInfo.ChiAttribute.Counteraction:
                    return 182;
                case Game.MsgServer.MsgChiInfo.ChiAttribute.AddAttack:
                    return 1850;
                case Game.MsgServer.MsgChiInfo.ChiAttribute.HPAdd:
                    return 3250;
                case Game.MsgServer.MsgChiInfo.ChiAttribute.AddMagicAttack:
                    return 2300;
                case Game.MsgServer.MsgChiInfo.ChiAttribute.AddMagicDefense:
                    return 275;
                case Game.MsgServer.MsgChiInfo.ChiAttribute.PhysicalDamageIncrease:
                    return 460;
                case Game.MsgServer.MsgChiInfo.ChiAttribute.MagicDamageIncrease:
                    return 275;
                case Game.MsgServer.MsgChiInfo.ChiAttribute.PhysicalDamageDecrease:
                    return 455;

                default: return 100;
            }
        }
        public static int ChiMinValues(Game.MsgServer.MsgChiInfo.ChiAttribute attribute)
        {
            switch (attribute)
            {
                case Game.MsgServer.MsgChiInfo.ChiAttribute.CriticalStrike: return 10;
                case Game.MsgServer.MsgChiInfo.ChiAttribute.SkillCriticalStrike: return 10;
                case Game.MsgServer.MsgChiInfo.ChiAttribute.Immunity: return 10;
                case Game.MsgServer.MsgChiInfo.ChiAttribute.Breakthrough: return 10;
                case Game.MsgServer.MsgChiInfo.ChiAttribute.Counteraction: return 10;
                case Game.MsgServer.MsgChiInfo.ChiAttribute.HPAdd: return 1000;
                case Game.MsgServer.MsgChiInfo.ChiAttribute.AddAttack: return 500;
                case Game.MsgServer.MsgChiInfo.ChiAttribute.AddMagicAttack: return 500;
                case Game.MsgServer.MsgChiInfo.ChiAttribute.AddMagicDefense: return 50;
                case Game.MsgServer.MsgChiInfo.ChiAttribute.PhysicalDamageIncrease: return 50;
                case Game.MsgServer.MsgChiInfo.ChiAttribute.PhysicalDamageDecrease: return 50;
                case Game.MsgServer.MsgChiInfo.ChiAttribute.MagicDamageIncrease: return 50;
                case Game.MsgServer.MsgChiInfo.ChiAttribute.MagicDamageDecrease: return 50;
                default: return 10;
            }
        }

        public unsafe struct ChiAttribute
        {
            public static ChiAttribute[] ShallowCopy(ChiAttribute[] item)
            {
                ChiAttribute[] obj = new ChiAttribute[item.Length];
                for (int i = 0; i < obj.Length; i++)
                {
                    obj[i] = (ChiAttribute)item[i].MemberwiseClone();
                }
                return obj;
            }
            public Qahr.Mobile.Game.MsgServer.MsgChiInfo.ChiAttribute Type;
            public UInt16 Value;
            public bool Locked;
            public ChiAttribute(Qahr.Mobile.Game.MsgServer.MsgChiInfo.ChiAttribute type, UInt16 value, bool locked)
            {
                Type = type;
                Value = value;
                Locked = locked;
            }
            public void Serialize(BinaryWriter writer)
            {
                writer.Write((Byte)Type);
                writer.Write(Value);
                writer.Write(Locked);
            }
            public void Deserialize(BinaryReader reader)
            {
                Type = (Qahr.Mobile.Game.MsgServer.MsgChiInfo.ChiAttribute)reader.ReadByte();
                Value = reader.ReadUInt16();
                Locked = reader.ReadBoolean();
            }
            public static implicit operator Int32(ChiAttribute attribute)
            {
                return (byte)attribute.Type * 10000 + attribute.Value;
            }
        }
        public static int MaxPower(ChiAttributeType power)
        {
            if (power == ChiAttributeType.CriticalStrike)
                return 200;
            else if (power == ChiAttributeType.SkillCriticalStrike)
                return 200;
            else if (power == ChiAttributeType.Immunity)
                return 200;
            else if (power == ChiAttributeType.Breakthrough)
                return 200;
            else if (power == ChiAttributeType.Counteraction)
                return 200;
            else if (power == ChiAttributeType.AddAttack)
                return 2000;
            else if (power == ChiAttributeType.MaxLife)
                return 3500;
            else if (power == ChiAttributeType.AddMagicAttack)
                return 2500;
            else if (power == ChiAttributeType.AddMagicDefense)
                return 250;
            else if (power == ChiAttributeType.FinalAttack)
                return 500;
            else if (power == ChiAttributeType.FinalMagicAttack)
                return 300;
            else if (power == ChiAttributeType.FinalDefense)
                return 500;
            else
                return 300;
        }
        public enum ChiAttributeType
        {
            None = 0,
            CriticalStrike,
            SkillCriticalStrike,
            Immunity,
            Breakthrough,
            Counteraction,
            MaxLife,
            AddAttack,
            AddMagicAttack,
            AddMagicDefense,
            FinalAttack,
            FinalMagicAttack,
            FinalDefense,
            FinalMagicDefense
        }
        public class ChiPower
        {
            public static readonly System.RandomLite Rand = new System.RandomLite();
            public Tuple<Role.Instance.Chi.ChiAttributeType, int>[] Fields { get; set; }
            public uint Exp;
            public ChiAttribute[] Attributes;
            public Database.FateExpTable.Value ChiAttributes
            {
                get
                {
                    return Database.FateExpTable.Values.Values.Where(i => i.ChiType == Type && Exp >= i.RequiredPoints).LastOrDefault();
                }
            }
            public Game.MsgServer.MsgChiInfo.ChiPowerType Type;//
            public int Score { get { return UnLocked ? (int)Attributes.Sum((x) => (uint)((x.Value - ChiMinValues(x.Type)) / (double)((ChiMaxValues(x.Type) - ChiMinValues(x.Type)) / 100d))) : 0; } }
            public bool UnLocked { get; set; }
            public string Name = "";
            public uint UID;
            public int Rank = 0;
            public ChiPower(Game.MsgServer.MsgChiInfo.ChiPowerType _Type = MsgChiInfo.ChiPowerType.None)
            {
                Attributes = new ChiAttribute[4];
                Type = _Type;
            }
            public void Reroll(Game.MsgServer.MsgChiInfo.LockedFlags Locked)
            {
                for (int x = 0; x < Attributes.Length; x++)
                {

                    if ((Locked & (Game.MsgServer.MsgChiInfo.LockedFlags)(1 << x)) != 0)
                    {
                        Attributes[x].Locked = true;
                        continue;
                    }


                    Attributes[x].Type = (MsgChiInfo.ChiAttribute)Rand.Next(1, 13);
                    // [hang-fix] cap the uniqueness re-roll: with more than 12 attributes a unique type is
                    // impossible, which used to spin forever. Normal play finds one in a few tries.
                    int _chiGuard = 1000;
                    do
                    {
                        Attributes[x].Type = (MsgChiInfo.ChiAttribute)Rand.Next(1, 13);
                    } while (Attributes.Where(i => i.Type == Attributes[x].Type).Count() > 1 && --_chiGuard > 0);

                    Attributes[x].Value = StatsRand(Attributes[x], false);
                    int max = ChiMaxValues(Attributes[x].Type);
                    if (100 * Attributes[x].Value / max >= 90)
                    {
                        using (var rec = new ServerSockets.RecycledPacket())
                        {
                            var stream = rec.GetStream();

                            stream.ChiMessageCreate(Type, Attributes[x], Name);
                            Server.SendGlobalPacket(stream);
                        }
                    }
                    Attributes[x].Locked = false;
                }
            }
            public ushort StatsRand(ChiAttribute attr, bool epic = false)//false
            {
                int value = 0;
                int max = ChiMaxValues(attr.Type),
                    min = ChiMinValues(attr.Type);
                if (epic) return (ushort)max;
            again:
                value = (ushort)Rand.Next(min, max);
                if (ChiAttributes != null && !Role.Core.Rate(ChiAttributes.StudyLuck / 100) || ChiAttributes == null)
                {
                    while (100 * value / max >= 70 && Role.Core.Rate(60) || 100 * value / max >= 90 && Role.Core.Rate(95))
                    {
                        value = (ushort)Rand.Next(min, max);
                    }
                }
                if (ChiAttributes != null)
                    if ((double)100 * (double)value / (double)max < ChiAttributes.MinScore)
                        goto again;
                return (ushort)value;
            }
            public override string ToString()
            {
                var file = new Database.DBActions.WriteLine('/').Add(UnLocked ? 1 : 0).Add(Exp);
                for (int x = 0; x < Attributes.Length; x++)
                {
                    file.Add((byte)Attributes[x].Type);
                    file.Add((ushort)Attributes[x].Value);
                    file.Add((byte)(Attributes[x].Locked ? 1 : 0));
                }
                return file.Close();
            }
            public void Load(string line, uint _UID, string _Name)
            {
                UID = _UID;
                Name = _Name;
                Database.DBActions.ReadLine reader = new Database.DBActions.ReadLine(line, '/');
                UnLocked = reader.Read((byte)0) == 1;
                if (UnLocked)
                {
                    Exp = reader.Read((uint)0);
                    for (int x = 0; x < Attributes.Length; x++)
                    {
                        Attributes[x].Type = (MsgChiInfo.ChiAttribute)reader.Read((byte)0);
                        Attributes[x].Value = reader.Read((ushort)0);
                        Attributes[x].Locked = reader.Read((byte)0) > 0;
                    }
                }
            }



            public bool ContainAtribut(MsgChiInfo.ChiAttribute type)
            {
                var arrt = Attributes.Where(p => p.Type == type);
                if (arrt.Count() > 0) return true;
                return false;
            }
        }


        public static ConcurrentDictionary<uint, Chi> ChiPool = new ConcurrentDictionary<uint, Chi>();

        public int ChiPoints = 0;

        public uint MaxChiPoints
        {
            get
            {
                uint points = 4000;
                foreach (var power in this)
                    if (power != null && power.UnLocked && power.ChiAttributes != null)
                        points += power.ChiAttributes.MaxChiPoints;
                return points;
            }
        }

        public string Name = "";
        public uint UID;


        public ChiPower Dragon;
        public ChiPower Phoenix;
        public ChiPower Turtle;
        public ChiPower Tiger;
        public long DragonTime = 0;
        public long PhoenixTime = 0;
        public long TurtleTime = 0;
        public long TigerTime = 0;
        public ChiAttribute[] DragonPowers = null;
        public ChiAttribute[] PhoenixPowers = null;
        public ChiAttribute[] TurtlePowers = null;
        public ChiAttribute[] TigerPowers = null;

        public Chi(uint _UID)
        {
            UID = _UID;
            ChiPoints = 4000;
            Dragon = new ChiPower(Game.MsgServer.MsgChiInfo.ChiPowerType.Dragon);
            Phoenix = new ChiPower(Game.MsgServer.MsgChiInfo.ChiPowerType.Phoenix);
            Turtle = new ChiPower(Game.MsgServer.MsgChiInfo.ChiPowerType.Turtle);
            Tiger = new ChiPower(Game.MsgServer.MsgChiInfo.ChiPowerType.Tiger);
        }

        public uint AllScore()
        {
            uint score = 0;
            foreach (var power in this)
            {
                score += (uint)power.Score;
            }
            return score;
        }

        public IEnumerator<ChiPower> GetEnumerator()
        {
            yield return Dragon;
            yield return Phoenix;
            yield return Tiger;
            yield return Turtle;
        }
        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
        {
            return this.GetEnumerator();
        }
        public unsafe void SendQueryUpdate(Client.GameClient user, Chi.ChiPower MyPower, ServerSockets.Packet stream)
        {
            var Info = new MsgRankProto()
            {
                Mode = (ulong)MsgGenericRanking.Action.QueryCount,
                ranktyp = (ulong)(Game.MsgServer.MsgGenericRanking.RankType)(60000000 + (byte)MyPower.Type),
                Page = 0,
                RegisteredCount = 0,
                Finish = 1,
            };
            Info.PlayerList.Add(new MsgRankListPlayers()
            {
                Rank = (ulong)(MyPower.Rank),
                Amount = (uint)MyPower.Score,
                UID1 = MyPower.UID,
                UID2 = MyPower.UID,
                name = MyPower.Name,
                name2 = MyPower.Name,
                Level = 0,
                Class = 0,
                Mesh = 0,
                Member10 = 0,
                Member11 = 0,
                AvatarFrame = 0
            });
            user.Send(stream.CreateRank(Info));
        }
        uint test = 0;

        public uint CriticalStrike
        {
            get
            {
                return test;
            }
            set
            {
                test = value;
            }
        }
        public uint SkillCriticalStrike { get; set; }
        public uint Immunity { get; set; }
        public uint Breakthrough { get; set; }
        public uint Counteraction { get; set; }
        public uint MaxLife { get; set; }
        public uint AddAttack { get; set; }
        public uint AddMagicAttack { get; set; }
        public uint AddMagicDefense { get; set; }
        public uint FinalAttack { get; set; }
        public uint FinalMagicAttack { get; set; }
        public uint FinalDefense { get; set; }
        public uint FinalMagicDefense { get; set; }

        public static object SynRoot = new object();
        public static void ComputeStatus(Chi ClientChi)
        {
            lock (SynRoot)
            {
                ClientChi.CriticalStrike = 0;
                ClientChi.SkillCriticalStrike = 0;
                ClientChi.Immunity = 0;
                ClientChi.Breakthrough = 0;
                ClientChi.Counteraction = 0;
                ClientChi.MaxLife = 0;
                ClientChi.AddAttack = 0;
                ClientChi.AddMagicAttack = 0;
                ClientChi.AddMagicDefense = 0;
                ClientChi.FinalAttack = 0;
                ClientChi.FinalDefense = 0;
                ClientChi.FinalMagicAttack = 0;
                ClientChi.FinalMagicDefense = 0;

                foreach (var power in ClientChi)
                {
                    if (!power.UnLocked)
                        continue;
                    var array = power.Attributes.Concat((new List<Role.Instance.Chi.ChiAttribute>().ToArray()));
                    foreach (var field in array)
                    {
                        int value = field.Value;
                        switch (field.Type)
                        {
                            case MsgChiInfo.ChiAttribute.AddAttack: ClientChi.AddAttack += field.Value; break;
                            case MsgChiInfo.ChiAttribute.AddMagicAttack: ClientChi.AddMagicAttack += field.Value; break;
                            case MsgChiInfo.ChiAttribute.AddMagicDefense: ClientChi.AddMagicDefense += field.Value; break;
                            case MsgChiInfo.ChiAttribute.Breakthrough: ClientChi.Breakthrough += field.Value; break;
                            case MsgChiInfo.ChiAttribute.Counteraction: ClientChi.Counteraction += field.Value; break;
                            case MsgChiInfo.ChiAttribute.CriticalStrike: ClientChi.CriticalStrike += (uint)(field.Value * 10); break;
                            case MsgChiInfo.ChiAttribute.PhysicalDamageIncrease: ClientChi.FinalAttack += field.Value; break;
                            case MsgChiInfo.ChiAttribute.PhysicalDamageDecrease: ClientChi.FinalDefense += field.Value; break;
                            case MsgChiInfo.ChiAttribute.MagicDamageIncrease: ClientChi.FinalMagicAttack += field.Value; break;
                            case MsgChiInfo.ChiAttribute.MagicDamageDecrease: ClientChi.FinalMagicDefense += field.Value; break;
                            case MsgChiInfo.ChiAttribute.Immunity: ClientChi.Immunity += (uint)(field.Value * 10); break;
                            case MsgChiInfo.ChiAttribute.HPAdd: ClientChi.MaxLife += field.Value; break;
                            case MsgChiInfo.ChiAttribute.SkillCriticalStrike: ClientChi.SkillCriticalStrike += (uint)(field.Value * 10); break;
                        }
                    }

                    if (power.Type == Game.MsgServer.MsgChiInfo.ChiPowerType.Dragon) // Chi Level 99
                    {
                        if (power.Exp == 6063000)
                        {

                            ClientChi.MaxLife += 2500; // Max~HP
                            ClientChi.FinalDefense += 500; // Final~P-Dmg
                            ClientChi.AddMagicDefense += 150; // M-Defense
                            ClientChi.FinalMagicDefense += 150; // Final~M-Dmg
                        }
                    }

                    if (power.Type == Game.MsgServer.MsgChiInfo.ChiPowerType.Phoenix) // Chi Level 99
                    {
                        if (power.Exp == 6063000)
                        {

                            ClientChi.AddAttack += 1500; // P~Attack
                            ClientChi.AddMagicAttack += 1500; // M~Attack
                            ClientChi.FinalAttack += 500; // Final~P~Attack
                            ClientChi.FinalMagicAttack += 150; // Final~M~Attack
                        }
                    }

                    if (power.Type == Game.MsgServer.MsgChiInfo.ChiPowerType.Tiger) // Chi Level 99
                    {
                        if (power.Exp == 6063000)
                        {

                            ClientChi.CriticalStrike += 8 * 100; // P~Strike
                            ClientChi.SkillCriticalStrike += 8 * 100; // M~Strike
                            ClientChi.Immunity += 4 * 100; // Immunity

                        }
                    }

                    if (power.Type == Game.MsgServer.MsgChiInfo.ChiPowerType.Turtle) // Chi Level 99
                    {
                        if (power.Exp == 6063000)
                        {

                            ClientChi.Breakthrough += 8 * 100; // Break
                            ClientChi.Counteraction += 8 * 100; // Anti~Break
                            ClientChi.Immunity += 4 * 100; // Immunity

                        }
                    }

                    if (power.Type == Game.MsgServer.MsgChiInfo.ChiPowerType.Dragon)
                    {
                        if (power.Rank >= 1 && power.Rank <= 3 || power.Score == 400)
                        {

                            ClientChi.MaxLife += 5000;
                            ClientChi.FinalAttack += 1000;
                            ClientChi.FinalMagicAttack += 300;
                            ClientChi.FinalMagicDefense += 300;
                        }
                        else if (power.Rank >= 4 && power.Rank <= 20)
                        {
                            #region MaxLife
                            if (power.ChiAttributes != null)
                            {
                                var GateAttributes = power.ChiAttributes.Attributes.FirstOrDefault(p => p.Type == Qahr.Mobile.Game.MsgServer.MsgChiInfo.ChiAttribute.HPAdd);
                                if (GateAttributes.Value > (uint)(3000 - (uint)((power.Rank - 4) * 62.5)))
                                    ClientChi.MaxLife += GateAttributes.Value;
                                else
                                    ClientChi.MaxLife += (uint)(3000 - (uint)((power.Rank - 4) * 62.5));
                            }
                            else
                                ClientChi.MaxLife += (uint)(3000 - (uint)((power.Rank - 4) * 62.5));
                            #endregion

                            #region FinalAttack
                            if (power.ChiAttributes != null)
                            {
                                var FinalAttack = power.ChiAttributes.Attributes.FirstOrDefault(p => p.Type == Qahr.Mobile.Game.MsgServer.MsgChiInfo.ChiAttribute.PhysicalDamageIncrease);
                                if (FinalAttack.Value > (uint)(600 - (uint)((power.Rank - 4) * 12.5)))
                                    ClientChi.FinalAttack += FinalAttack.Value;
                                else
                                    ClientChi.FinalAttack += (uint)(600 - (uint)((power.Rank - 4) * 12.5));
                            }
                            else
                                ClientChi.FinalAttack += (uint)(600 - (uint)((power.Rank - 4) * 12.5));
                            #endregion

                            #region FinalMagicAttack
                            if (power.ChiAttributes != null)
                            {
                                var GateAttributes = power.ChiAttributes.Attributes.FirstOrDefault(p => p.Type == Qahr.Mobile.Game.MsgServer.MsgChiInfo.ChiAttribute.MagicDamageIncrease);
                                if (GateAttributes.Value > (uint)(200 - (uint)((power.Rank - 4) * 3)))
                                    ClientChi.FinalMagicAttack += GateAttributes.Value;
                                else
                                    ClientChi.FinalMagicAttack += (uint)(200 - (uint)((power.Rank - 4) * 3));
                            }
                            else
                                ClientChi.FinalMagicAttack += (uint)(200 - (uint)((power.Rank - 4) * 3));
                            #endregion

                            #region FinalMagicDefense
                            if (power.ChiAttributes != null)
                            {
                                var GateAttributes = power.ChiAttributes.Attributes.FirstOrDefault(p => p.Type == Qahr.Mobile.Game.MsgServer.MsgChiInfo.ChiAttribute.MagicDamageDecrease);
                                if (GateAttributes.Value > (uint)(200 - (uint)((power.Rank - 4) * 3)))
                                    ClientChi.FinalMagicDefense += GateAttributes.Value;
                                else
                                    ClientChi.FinalMagicDefense += (uint)(200 - (uint)((power.Rank - 4) * 3));
                            }
                            else
                                ClientChi.FinalMagicDefense += (uint)(200 - (uint)((power.Rank - 4) * 3));
                            #endregion

                        }
                        else if (power.Rank >= 21 && power.Rank <= 50)
                        {
                            #region MaxLife
                            if (power.ChiAttributes != null)
                            {
                                var GateAttributes = power.ChiAttributes.Attributes.FirstOrDefault(p => p.Type == Qahr.Mobile.Game.MsgServer.MsgChiInfo.ChiAttribute.HPAdd);

                                if (GateAttributes.Value > 1500)
                                    ClientChi.MaxLife += GateAttributes.Value;
                                else
                                    ClientChi.MaxLife += 1500;
                            }
                            else
                                ClientChi.MaxLife += 1500;
                            #endregion

                            #region FinalAttack
                            if (power.ChiAttributes != null)
                            {
                                var GateAttributes = power.ChiAttributes.Attributes.FirstOrDefault(p => p.Type == Qahr.Mobile.Game.MsgServer.MsgChiInfo.ChiAttribute.PhysicalDamageIncrease);
                                if (GateAttributes.Value > 300)
                                    ClientChi.FinalAttack += GateAttributes.Value;
                                else
                                    ClientChi.FinalAttack += 300;
                            }
                            else
                                ClientChi.FinalAttack += 300;
                            #endregion

                            #region FinalMagicAttack
                            if (power.ChiAttributes != null)
                            {
                                var GateAttributes = power.ChiAttributes.Attributes.FirstOrDefault(p => p.Type == Qahr.Mobile.Game.MsgServer.MsgChiInfo.ChiAttribute.MagicDamageIncrease);
                                if (GateAttributes.Value > 100)
                                    ClientChi.FinalMagicAttack += GateAttributes.Value;
                                else
                                    ClientChi.FinalMagicAttack += 100;
                            }
                            else
                                ClientChi.FinalMagicAttack += 100;
                            #endregion

                            #region FinalMagicDefense

                            if (power.ChiAttributes != null)
                            {
                                var GateAttributes = power.ChiAttributes.Attributes.FirstOrDefault(p => p.Type == Qahr.Mobile.Game.MsgServer.MsgChiInfo.ChiAttribute.MagicDamageDecrease);
                                if (GateAttributes.Value > 100)
                                    ClientChi.FinalMagicDefense += GateAttributes.Value;
                                else
                                    ClientChi.FinalMagicDefense += 100;
                            }
                            else
                                ClientChi.FinalMagicDefense += 100;
                            #endregion

                        }
                    }
                    else if (power.Type == Game.MsgServer.MsgChiInfo.ChiPowerType.Phoenix)
                    {
                        if (power.Rank >= 1 && power.Rank <= 3 || power.Score == 400)
                        {
                            ClientChi.AddAttack += 3000;
                            ClientChi.AddMagicAttack += 3000;
                            ClientChi.FinalAttack += 1000;
                            ClientChi.FinalMagicAttack += 300;
                        }
                        else if (power.Rank >= 4 && power.Rank <= 20)
                        {
                            if (power.ChiAttributes != null)
                            {
                                #region AddAttack
                                var GateAttributes = power.ChiAttributes.Attributes.FirstOrDefault(p => p.Type == Qahr.Mobile.Game.MsgServer.MsgChiInfo.ChiAttribute.AddAttack);

                                if (GateAttributes.Value > (uint)(2000 - (uint)((power.Rank - 4) * 31.5)))
                                    ClientChi.AddAttack += GateAttributes.Value;
                                else
                                    ClientChi.AddAttack += (uint)(2000 - (uint)((power.Rank - 4) * 31.5));
                                #endregion

                                #region MagicAttack

                                GateAttributes = power.ChiAttributes.Attributes.FirstOrDefault(p => p.Type == Qahr.Mobile.Game.MsgServer.MsgChiInfo.ChiAttribute.AddMagicAttack);
                                if (GateAttributes.Value > (uint)(2000 - (uint)((power.Rank - 4) * 31.5)))
                                    ClientChi.AddMagicAttack += GateAttributes.Value;
                                else
                                    ClientChi.AddMagicAttack += (uint)(2000 - (uint)((power.Rank - 4) * 31.5));
                                #endregion

                                #region FinalAttack
                                GateAttributes = power.ChiAttributes.Attributes.FirstOrDefault(p => p.Type == Qahr.Mobile.Game.MsgServer.MsgChiInfo.ChiAttribute.PhysicalDamageIncrease);
                                if (GateAttributes.Value > (uint)(600 - (uint)((power.Rank - 4) * 12.5)))
                                    ClientChi.FinalAttack += GateAttributes.Value;
                                else
                                    ClientChi.FinalAttack += (uint)(600 - (uint)((power.Rank - 4) * 12.5));
                                #endregion

                                #region FinalMagicAttack
                                GateAttributes = power.ChiAttributes.Attributes.FirstOrDefault(p => p.Type == Qahr.Mobile.Game.MsgServer.MsgChiInfo.ChiAttribute.MagicDamageIncrease);
                                if (GateAttributes.Value > (uint)(200 - (uint)((power.Rank - 4) * 3)))
                                    ClientChi.FinalMagicAttack += GateAttributes.Value;
                                else
                                    ClientChi.FinalMagicAttack += (uint)(200 - (uint)((power.Rank - 4) * 3));
                                #endregion
                            }
                            else
                            {
                                ClientChi.AddAttack += (uint)(2000 - (uint)((power.Rank - 4) * 31.5));
                                ClientChi.AddMagicAttack += (uint)(2000 - (uint)((power.Rank - 4) * 31.5));
                                ClientChi.FinalAttack += (uint)(600 - (uint)((power.Rank - 4) * 12.5));
                                ClientChi.FinalMagicAttack += (uint)(200 - (uint)((power.Rank - 4) * 3));
                            }
                        }
                        else if (power.Rank >= 21 && power.Rank <= 50)
                        {
                            if (power.ChiAttributes != null)
                            {
                                #region AddAttack
                                var GateAttributes = power.ChiAttributes.Attributes.FirstOrDefault(p => p.Type == Qahr.Mobile.Game.MsgServer.MsgChiInfo.ChiAttribute.AddAttack);

                                if (GateAttributes.Value > 1000)
                                    ClientChi.AddAttack += GateAttributes.Value;
                                else
                                    ClientChi.AddAttack += (uint)1000;
                                #endregion

                                #region MagicAttack

                                GateAttributes = power.ChiAttributes.Attributes.FirstOrDefault(p => p.Type == Qahr.Mobile.Game.MsgServer.MsgChiInfo.ChiAttribute.AddMagicAttack);
                                if (GateAttributes.Value > 2000)
                                    ClientChi.AddMagicAttack += GateAttributes.Value;
                                else
                                    ClientChi.AddMagicAttack += (uint)2000;
                                #endregion

                                #region FinalAttack
                                GateAttributes = power.ChiAttributes.Attributes.FirstOrDefault(p => p.Type == Qahr.Mobile.Game.MsgServer.MsgChiInfo.ChiAttribute.PhysicalDamageIncrease);
                                if (GateAttributes.Value > 1000)
                                    ClientChi.FinalAttack += GateAttributes.Value;
                                else
                                    ClientChi.FinalAttack += (uint)1000;
                                #endregion

                                #region FinalMagicAttack
                                GateAttributes = power.ChiAttributes.Attributes.FirstOrDefault(p => p.Type == Qahr.Mobile.Game.MsgServer.MsgChiInfo.ChiAttribute.MagicDamageIncrease);
                                if (GateAttributes.Value > (uint)300)
                                    ClientChi.FinalMagicAttack += GateAttributes.Value;
                                else
                                    ClientChi.FinalMagicAttack += (uint)300;
                                #endregion
                            }
                            else
                            {
                                ClientChi.AddAttack += 1000;
                                ClientChi.AddMagicAttack += 2000;
                                ClientChi.FinalAttack += 1000;
                                ClientChi.FinalMagicAttack += 300;
                            }



                        }
                    }
                    else if (power.Type == Game.MsgServer.MsgChiInfo.ChiPowerType.Tiger)
                    {
                        if (power.Rank >= 1 && power.Rank <= 3 || power.Score == 400)
                        {
                            ClientChi.CriticalStrike += 15 * 100;
                            ClientChi.SkillCriticalStrike += 15 * 100;
                            ClientChi.Immunity += 8 * 100;
                        }
                        else if (power.Rank >= 4 && power.Rank <= 20)
                        {
                            if (power.ChiAttributes != null)
                            {
                                #region CriticalStrike
                                var GateAttributes = power.ChiAttributes.Attributes.FirstOrDefault(p => p.Type == Qahr.Mobile.Game.MsgServer.MsgChiInfo.ChiAttribute.CriticalStrike);

                                if ((GateAttributes.Value * 10) > (uint)(11 * 100 - (uint)((power.Rank - 4) * 17)))
                                    ClientChi.CriticalStrike += (uint)(GateAttributes.Value * 10);
                                else
                                    ClientChi.CriticalStrike += (uint)(11 * 100 - (uint)((power.Rank - 4) * 17));
                                #endregion

                                #region SkillCriticalStrike

                                GateAttributes = power.ChiAttributes.Attributes.FirstOrDefault(p => p.Type == Qahr.Mobile.Game.MsgServer.MsgChiInfo.ChiAttribute.SkillCriticalStrike);
                                if (GateAttributes.Value > (uint)(11 * 100 - (uint)((power.Rank - 4) * 17)))
                                    ClientChi.SkillCriticalStrike += GateAttributes.Value;
                                else
                                    ClientChi.SkillCriticalStrike += (uint)(uint)(11 * 100 - (uint)((power.Rank - 4) * 17));
                                #endregion

                                #region Immunity
                                GateAttributes = power.ChiAttributes.Attributes.FirstOrDefault(p => p.Type == Qahr.Mobile.Game.MsgServer.MsgChiInfo.ChiAttribute.Immunity);
                                if ((GateAttributes.Value * 10) > (uint)(5 * 100 - (uint)((power.Rank - 4) * 13)))
                                    ClientChi.Immunity += GateAttributes.Value;
                                else
                                    ClientChi.Immunity += (uint)(uint)(5 * 100 - (uint)((power.Rank - 4) * 13));
                                #endregion
                            }
                            else
                            {
                                ClientChi.CriticalStrike += (uint)(11 * 100 - (uint)((power.Rank - 4) * 17));
                                ClientChi.SkillCriticalStrike += (uint)(11 * 100 - (uint)((power.Rank - 4) * 17));
                                ClientChi.Immunity += (uint)(5 * 100 - (uint)((power.Rank - 4) * 13));
                            }

                        }
                        else if (power.Rank >= 21 && power.Rank <= 50)
                        {
                            if (power.ChiAttributes != null)
                            {
                                #region CriticalStrike
                                var GateAttributes = power.ChiAttributes.Attributes.FirstOrDefault(p => p.Type == Qahr.Mobile.Game.MsgServer.MsgChiInfo.ChiAttribute.CriticalStrike);

                                if ((GateAttributes.Value * 10) > 5 * 100)
                                    ClientChi.CriticalStrike += (uint)(GateAttributes.Value * 10);
                                else
                                    ClientChi.CriticalStrike += (uint)(5 * 100);
                                #endregion

                                #region SkillCriticalStrike

                                GateAttributes = power.ChiAttributes.Attributes.FirstOrDefault(p => p.Type == Qahr.Mobile.Game.MsgServer.MsgChiInfo.ChiAttribute.SkillCriticalStrike);
                                if (GateAttributes.Value > (uint)5 * 100)
                                    ClientChi.SkillCriticalStrike += GateAttributes.Value;
                                else
                                    ClientChi.SkillCriticalStrike += (uint)(5 * 100);
                                #endregion

                                #region Immunity
                                GateAttributes = power.ChiAttributes.Attributes.FirstOrDefault(p => p.Type == Qahr.Mobile.Game.MsgServer.MsgChiInfo.ChiAttribute.Immunity);
                                if ((GateAttributes.Value * 10) > 2 * 100)
                                    ClientChi.Immunity += GateAttributes.Value;
                                else
                                    ClientChi.Immunity += 2 * 100;
                                #endregion
                            }
                            else
                            {
                                ClientChi.CriticalStrike += 5 * 100;
                                ClientChi.SkillCriticalStrike += 5 * 100;
                                ClientChi.Immunity += 2 * 100;
                            }

                        }
                    }
                    else if (power.Type == Game.MsgServer.MsgChiInfo.ChiPowerType.Turtle)
                    {
                        if (power.Rank >= 1 && power.Rank <= 3 || power.Score == 400)
                        {
                            ClientChi.Breakthrough += 15 * 10;
                            ClientChi.Counteraction += 15 * 10;
                            ClientChi.Immunity += 8 * 100;
                        }
                        else if (power.Rank >= 4 && power.Rank <= 20)
                        {
                            if (power.ChiAttributes != null)
                            {
                                #region Counteraction

                                var GateAttributes = power.ChiAttributes.Attributes.FirstOrDefault(p => p.Type == Qahr.Mobile.Game.MsgServer.MsgChiInfo.ChiAttribute.Breakthrough);
                                if (GateAttributes.Value > (uint)((11 * 100 - (uint)((power.Rank - 4) * 17)) / 10))
                                    ClientChi.Breakthrough += GateAttributes.Value;
                                else
                                    ClientChi.Breakthrough += (uint)((11 * 100 - (uint)((power.Rank - 4) * 17)) / 10);
                                #endregion

                                #region Counteraction

                                GateAttributes = power.ChiAttributes.Attributes.FirstOrDefault(p => p.Type == Qahr.Mobile.Game.MsgServer.MsgChiInfo.ChiAttribute.Counteraction);
                                if (GateAttributes.Value > (uint)((11 * 100 - (uint)((power.Rank - 4) * 17)) / 10))
                                    ClientChi.Counteraction += GateAttributes.Value;
                                else
                                    ClientChi.Counteraction += (uint)((11 * 100 - (uint)((power.Rank - 4) * 17)) / 10);
                                #endregion

                                #region Immunity
                                GateAttributes = power.ChiAttributes.Attributes.FirstOrDefault(p => p.Type == Qahr.Mobile.Game.MsgServer.MsgChiInfo.ChiAttribute.Immunity);
                                if ((GateAttributes.Value * 10) > (uint)(5 * 100 - (uint)((power.Rank - 4) * 13)))
                                    ClientChi.Immunity += GateAttributes.Value;
                                else
                                    ClientChi.Immunity += (uint)(uint)(5 * 100 - (uint)((power.Rank - 4) * 13));
                                #endregion
                            }
                            else
                            {
                                ClientChi.Breakthrough += (uint)((11 * 100 - (uint)((power.Rank - 4) * 17)) / 10);
                                ClientChi.Counteraction += (uint)((11 * 100 - (uint)((power.Rank - 4) * 17)) / 10);
                                ClientChi.Immunity += (uint)(5 * 100 - (uint)((power.Rank - 4) * 13));
                            }

                        }
                        else if (power.Rank >= 21 && power.Rank <= 50)
                        {
                            if (power.ChiAttributes != null)
                            {
                                #region Breakthrough
                                var GateAttributes = power.ChiAttributes.Attributes.FirstOrDefault(p => p.Type == Qahr.Mobile.Game.MsgServer.MsgChiInfo.ChiAttribute.Breakthrough);

                                if ((GateAttributes.Value) > 5 * 10)
                                    ClientChi.Breakthrough += (uint)(GateAttributes.Value);
                                else
                                    ClientChi.Breakthrough += (uint)(5 * 10);
                                #endregion

                                #region SkillCriticalStrike

                                GateAttributes = power.ChiAttributes.Attributes.FirstOrDefault(p => p.Type == Qahr.Mobile.Game.MsgServer.MsgChiInfo.ChiAttribute.Counteraction);
                                if (GateAttributes.Value > (uint)5 * 10)
                                    ClientChi.Counteraction += GateAttributes.Value;
                                else
                                    ClientChi.Counteraction += (uint)(5 * 10);
                                #endregion

                                #region Immunity
                                GateAttributes = power.ChiAttributes.Attributes.FirstOrDefault(p => p.Type == Qahr.Mobile.Game.MsgServer.MsgChiInfo.ChiAttribute.Immunity);
                                if ((GateAttributes.Value * 10) > 2 * 100)
                                    ClientChi.Immunity += GateAttributes.Value;
                                else
                                    ClientChi.Immunity += 2 * 100;
                                #endregion
                            }
                            else
                            {
                                ClientChi.Breakthrough += 5 * 10;
                                ClientChi.Counteraction += 5 * 10;
                                ClientChi.Immunity += 2 * 100;
                            }

                        }
                    }
                }

            }
        }

    }
}
