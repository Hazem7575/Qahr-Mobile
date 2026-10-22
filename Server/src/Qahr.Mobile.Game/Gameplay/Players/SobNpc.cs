using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Qahr.Mobile.Game.MsgServer;
using ProtoBuf;

namespace Qahr.Mobile.Role
{

    public unsafe class SobNpc : IMapObj
    {
        [ProtoContract]
        public class SobNpcSpawnProto
        {
            [ProtoMember(1, IsRequired = true)]
            public uint UID;
            [ProtoMember(2, IsRequired = true)]
            public uint OwnerUID;
            [ProtoMember(3, IsRequired = true)]
            public uint Unknown2;
            [ProtoMember(4, IsRequired = true)]
            public uint MaxHitpoints;
            [ProtoMember(5, IsRequired = true)]
            public uint Hitpoints;
            [ProtoMember(6, IsRequired = true)]
            public ushort X;
            [ProtoMember(7, IsRequired = true)]
            public ushort Y;
            [ProtoMember(8, IsRequired = true)]
            public ushort Mesh;
            [ProtoMember(9, IsRequired = true)]
            public byte Type;
            [ProtoMember(10, IsRequired = true)]
            public ushort Sort;
            [ProtoMember(12)]
            public string Name;
        }

        public enum StaticMesh : uint
        {
            Vendor = 406,
            LeftGate = 241,
            OpenLeftGate = 251,
            RightGate = 277,
            OpenRightGate = 287,
            Pole = 1137,

        }
        public Role.Player.SpawnPacketProto botbuffer = null;
        public Role.Statue statue = null;
        public bool AllowDynamic { get; set; }
        public Role.StatusFlagsBigVector32 BitVector;
        public uint IndexInScreen { get; set; }
        public bool IsStatue
        {
            get { return statue != null; }
        }
        public SobNpc(Role.Statue _statue)
        {
            statue = _statue;
            BitVector = new StatusFlagsBigVector32(32 * 18);
        }

        public Game.Booth Booth;
        public SobNpc()
        {
            AllowDynamic = false;
            BitVector = new StatusFlagsBigVector32(32 * 18);
        }
        public const byte SeedDistrance = 19;//17
        public bool IsTrap() { return false; }
        public uint UID { get; set; }
        public int MaxHitPoints { get; set; }
        int Hit;
        public int HitPoints
        {
            get { return Hit; }
            set
            {
                Hit = value;
            }

        }

        public ushort X { get; set; }
        public ushort Y { get; set; }
        public StaticMesh Mesh;
        public Flags.NpcType Type;
        public ushort Sort;
        public string Name { get; set; }

        public uint Map { get; set; }
        public uint DynamicID { get; set; }

        public bool Alive { get { return HitPoints > 0; } }
        public MapObjectType ObjType { get; set; }

        public Client.GameClient OwnerVendor;

        public void RemoveRole(IMapObj obj)
        {

        }
        public unsafe void Send(byte[] packet)
        {

        }
        public unsafe void Send(ServerSockets.Packet msg)
        {

        }
        public bool AddFlag(Game.MsgServer.MsgUpdate.Flags Flag, int Secounds, bool RemoveOnDead, int StampSecounds = 0, uint showamount = 0, uint amount = 0)
        {
            if (!BitVector.ContainFlag((int)Flag))
            {
                BitVector.TryAdd((int)Flag, Secounds, RemoveOnDead, StampSecounds);
                UpdateFlagScreen();
                return true;
            }
            return false;
        }
        public bool RemoveFlag(Game.MsgServer.MsgUpdate.Flags Flag)
        {
            if (BitVector.ContainFlag((int)Flag))
            {
                BitVector.TryRemove((int)Flag);
                UpdateFlagScreen();

                return true;
            }
            return false;
        }
        public bool ContainFlag(Game.MsgServer.MsgUpdate.Flags Flag)
        {
            return BitVector.ContainFlag((int)Flag);
        }
        public void UpdateFlagScreen()
        {
            using (var rec = new ServerSockets.RecycledPacket())
            {
                var stream = rec.GetStream();

                Game.MsgServer.MsgUpdate upd = new Game.MsgServer.MsgUpdate(stream, UID, 1);
                stream = upd.Append(stream, MsgUpdate.DataType.StatusFlag, BitVector.bits);
                stream = upd.GetArray(stream);

                foreach (var user in Pool.GamePoll.Values)
                {
                    if (user.Player.Map == Map)
                        user.Send(stream);
                }
            }
        }

        public unsafe void Die(ServerSockets.Packet stream, Client.GameClient killer)
        {
            if (HitPoints == 0)
                return;
            if (Map == 2071)
            {
                HitPoints = MaxHitPoints;
                InteractQuery action = new InteractQuery()
                {
                    UID = killer.Player.UID,
                    X = X,
                    Y = Y,
                    AtkType = (ushort)MsgAttackPacket.AttackID.Death,
                    SpellLevel = killer.Player.KillCounter,
                    SpellID = (ushort)(Database.ItemType.IsBow(killer.Equipment.RightWeapon) ? 5 : 1),
                    OpponentUID = UID,
                };


                killer.Player.View.SendView(stream.InteractionCreate(action), true);
                killer.Player.FlagsPointss += 20;
                Game.MsgTournaments.MsgSchedules.SendSysMesage("" + killer.Player.Name + " killed Flag Base and got 20 Pts.", MsgMessage.ChatMode.SlideCrosTheServer, MsgMessage.MsgColor.white);
                Game.MsgServer.MsgUpdate upd = new Game.MsgServer.MsgUpdate(stream, UID, 2);
                stream = upd.Append(stream, Game.MsgServer.MsgUpdate.DataType.Hitpoints, (long)HitPoints);
                stream = upd.Append(stream, Game.MsgServer.MsgUpdate.DataType.MaxHitpoints, (long)MaxHitPoints);
                stream = upd.GetArray(stream);
                killer.Player.View.SendView(stream, true);
                return;
            }
            if (Map == 1039)
            {
                HitPoints = MaxHitPoints;
                InteractQuery action = new InteractQuery()
                {
                    UID = killer.Player.UID,
                    X = X,
                    Y = Y,
                    AtkType = (ushort)MsgAttackPacket.AttackID.Death,
                    SpellLevel = killer.Player.KillCounter,
                    SpellID = (ushort)(Database.ItemType.IsBow(killer.Equipment.RightWeapon) ? 5 : 1),
                    OpponentUID = UID,
                };
                killer.Player.View.SendView(stream.InteractionCreate(action), true);


                Game.MsgServer.MsgUpdate upd = new Game.MsgServer.MsgUpdate(stream, UID, 2);
                stream = upd.Append(stream, Game.MsgServer.MsgUpdate.DataType.Hitpoints, (long)HitPoints);
                stream = upd.Append(stream, Game.MsgServer.MsgUpdate.DataType.MaxHitpoints, (long)MaxHitPoints);
                stream = upd.GetArray(stream);
                killer.Player.View.SendView(stream, true);
                return;
            }
            if (IsStatue)
            {
                HitPoints = 0;
                Role.Statue.RemoveStatue(stream, killer, UID, this);
                return;
            }
            if (UID == Game.MsgTournaments.MsgSchedules.GuildWar.Furnitures[StaticMesh.RightGate].UID)
            {
                if (Game.MsgTournaments.MsgSchedules.GuildWar.Winner != null)
                {
                    Instance.Guild guild;
                    if (Instance.Guild.GuildPoll.TryGetValue(Game.MsgTournaments.MsgSchedules.GuildWar.Winner.GuildID, out guild))
                    {
#if Arabic
                         guild.SendMessajGuild("{Guild War) The Right Gate has been breached!");
#else
                        guild.SendMessajGuild("(Guild War) The Right Gate has been breached!");
#endif

                    }
                }
                //   Hit = MaxHitPoints;
                Mesh = StaticMesh.OpenRightGate;
                Game.MsgServer.MsgUpdate upd = new Game.MsgServer.MsgUpdate(stream, UID, 1);
                stream = upd.Append(stream, Game.MsgServer.MsgUpdate.DataType.Mesh, (long)Mesh);
                stream = upd.Append(stream, 0, HitPoints);
                stream = upd.GetArray(stream);

                foreach (var client in Pool.GamePoll.Values)
                {
                    if (client.Player.Map == Map)
                    {
                        if (Role.Core.GetDistance(client.Player.X, client.Player.Y, X, Y) <= Role.SobNpc.SeedDistrance)
                        {
                            client.Send(stream);
                        }

                    }
                }
            }
            if (UID == Game.MsgTournaments.MsgSchedules.GuildWar.Furnitures[StaticMesh.LeftGate].UID)
            {
                if (Game.MsgTournaments.MsgSchedules.GuildWar.Winner != null)
                {
                    Instance.Guild guild;
                    if (Instance.Guild.GuildPoll.TryGetValue(Game.MsgTournaments.MsgSchedules.GuildWar.Winner.GuildID, out guild))
                    {
#if Arabic
                            guild.SendMessajGuild("(Guild War) The Left Gate has been breached!");
#else
                        guild.SendMessajGuild("(Guild War) The Left Gate has been breached!");
#endif

                    }
                }
                //   Hit = MaxHitPoints;
                Mesh = StaticMesh.OpenLeftGate;

                Game.MsgServer.MsgUpdate upd = new Game.MsgServer.MsgUpdate(stream, UID, 1);
                stream = upd.Append(stream, 0, HitPoints);
                stream = upd.Append(stream, Game.MsgServer.MsgUpdate.DataType.Mesh, (long)Mesh);
                stream = upd.GetArray(stream);

                foreach (var client in Pool.GamePoll.Values)
                {
                    if (client.Player.Map == Map)
                    {
                        if (Role.Core.GetDistance(client.Player.X, client.Player.Y, X, Y) <= Role.SobNpc.SeedDistrance)
                        {
                            client.Send(stream);
                        }

                    }
                }
            }
            if (UID == Game.MsgTournaments.MsgSchedules.SuperGuildWar.Furnitures[Game.MsgTournaments.MsgSuperGuildWar.FurnituresType.CastleLeftGate].UID
                || UID == Game.MsgTournaments.MsgSchedules.SuperGuildWar.Furnitures[Game.MsgTournaments.MsgSuperGuildWar.FurnituresType.CastleRightGate].UID)
            {
                Mesh = StaticMesh.OpenRightGate;
                Game.MsgServer.MsgUpdate upd = new Game.MsgServer.MsgUpdate(stream, UID, 1);
                stream = upd.Append(stream, Game.MsgServer.MsgUpdate.DataType.Mesh, (long)Mesh);
                stream = upd.GetArray(stream);
                foreach (var client in Pool.GamePoll.Values.Where(p => p.Fake == false))
                {
                    if (client.Player.Map == Map)
                    {
                        if (Role.Core.GetDistance(client.Player.X, client.Player.Y, X, Y) <= Role.SobNpc.SeedDistrance)
                        {
                            client.Send(stream);
                        }

                    }
                }
            }
            if (UID == Game.MsgTournaments.MsgSchedules.SuperGuildWar.Furnitures[Game.MsgTournaments.MsgSuperGuildWar.FurnituresType.CenterGate].UID
               || UID == Game.MsgTournaments.MsgSchedules.SuperGuildWar.Furnitures[Game.MsgTournaments.MsgSuperGuildWar.FurnituresType.LeftGate].UID
                || UID == Game.MsgTournaments.MsgSchedules.SuperGuildWar.Furnitures[Game.MsgTournaments.MsgSuperGuildWar.FurnituresType.RightGate].UID)
            {
                Mesh = StaticMesh.OpenLeftGate;
                Game.MsgServer.MsgUpdate upd = new Game.MsgServer.MsgUpdate(stream, UID, 1);
                stream = upd.Append(stream, Game.MsgServer.MsgUpdate.DataType.Mesh, (long)Mesh);
                stream = upd.GetArray(stream);
                foreach (var client in Pool.GamePoll.Values.Where(p => p.Fake == false))
                {
                    if (client.Player.Map == Map)
                    {
                        if (Role.Core.GetDistance(client.Player.X, client.Player.Y, X, Y) <= Role.SobNpc.SeedDistrance)
                        {
                            client.Send(stream);
                        }

                    }
                }
            }





            //if (UID == 890)
            //{
            //    uint Damage = (uint)HitPoints;
            //    if (HitPoints > 0)
            //    {
            //        HitPoints = 0;
            //    }
            //    Game.MsgTournaments.MsgSchedules.ClanWar.CurentWar.UpdateScore(killer.Player, Damage);
            //}

            else if (UID == Game.MsgTournaments.MsgSchedules.SuperGuildWar.Furnitures[Game.MsgTournaments.MsgSuperGuildWar.FurnituresType.Pole].UID)
            {
                uint Damage = (uint)HitPoints;
                if (HitPoints > 0)
                {
                    HitPoints = 0;
                }
                Game.MsgTournaments.MsgSchedules.SuperGuildWar.UpdateScore(killer.Player, Damage);
            }




            //else if (UID == Game.MsgTournaments.MsgSchedules.Guild6PoleWar6.Furnitures[StaticMesh.Pole].UID)
            //{
            //    uint Damage = (uint)HitPoints;

            //    if (HitPoints > 0)
            //        HitPoints = 0;

            //    Game.MsgTournaments.MsgSchedules.Guild6PoleWar6.UpdateScore(stream, Damage, killer.Player.MyGuild);
            //}

            //else if (UID == Game.MsgTournaments.MsgSchedules.TopWarScore.Furnitures[StaticMesh.Pole].UID)
            //{
            //    uint Damage = (uint)HitPoints;

            //    if (HitPoints > 0)
            //        HitPoints = 0;

            //    Game.MsgTournaments.MsgSchedules.TopWarScore.UpdateScore(stream, Damage, killer.Player.MyGuild);
            //}
            //else if (UID == 13140)
            //{
            //    uint Damage = (uint)HitPoints;
            //    if (HitPoints > 0)
            //    {
            //        HitPoints = 0;
            //    }
            //    Game.MsgTournaments.MsgSchedules.UnionWar.UpdateScore(killer.Player, Damage);
            //}


            //else if (UID == Game.MsgTournaments.MsgSchedules.EmperorWar.Furnitures[StaticMesh.Pole].UID)
            //{
            //    uint Damage = (uint)HitPoints;

            //    if (HitPoints > 0)
            //        HitPoints = 0;

            //    Game.MsgTournaments.MsgSchedules.EmperorWar.UpdateScore(stream, Damage, killer.Player.MyGuild);
            //}

            else if (UID == Game.MsgTournaments.MsgWarOfPlayers.Furnitures[StaticMesh.Pole].UID)
            {
                uint Damage = (uint)HitPoints;

                if (HitPoints > 0)
                    HitPoints = 0;

                Game.MsgTournaments.MsgWarOfPlayers.UpdateScore(stream, Damage, killer.Player);
            }

            //else if (UID == Game.MsgTournaments.MsgNewKingsWar.Furnitures[StaticMesh.Pole].UID)
            //{
            //    uint Damage = (uint)HitPoints;

            //    if (HitPoints > 0)
            //        HitPoints = 0;

            //    Game.MsgTournaments.MsgNewKingsWar.UpdateScore(stream, Damage, killer.Player);
            //}

            else if (UID == Game.MsgTournaments.MsgSchedules.GuildWar.Furnitures[Role.SobNpc.StaticMesh.Pole].UID)
            {
                uint Damage = (uint)HitPoints;
                if (HitPoints > 0)
                {
                    HitPoints = 0;
                }
                Game.MsgTournaments.MsgSchedules.GuildWar.UpdateScore(killer.Player, Damage);
            }

            else if (Game.MsgTournaments.MsgSchedules.CaptureTheFlag.Bases.ContainsKey(UID))
            {
                uint Damage = (uint)HitPoints;
                if (HitPoints > 0)
                {
                    HitPoints = 0;
                }
                Game.MsgTournaments.MsgSchedules.CaptureTheFlag.UpdateFlagScore(killer.Player, this, 0, stream);

            }
            else if (UID == Game.MsgTournaments.MsgSchedules.EliteGuildWar.Furnitures[StaticMesh.Pole].UID)
            {
                uint Damage = (uint)HitPoints;

                if (HitPoints > 0)
                    HitPoints = 0;

                Game.MsgTournaments.MsgSchedules.EliteGuildWar.UpdateScore(stream, Damage, killer.Player.MyGuild);
            }

            //else if (UID == Game.MsgTournaments.MsgSchedules.ClanTwin.Furnitures[StaticMesh.Pole].UID)
            //{
            //    uint Damage = (uint)HitPoints;

            //    if (HitPoints > 0)
            //        HitPoints = 0;

            //    Game.MsgTournaments.MsgSchedules.ClanTwin.UpdateScore(stream, Damage, killer.Player.MyClan);
            //}
            //else if (UID == Game.MsgTournaments.MsgSchedules.ClanPhoenix.Furnitures[StaticMesh.Pole].UID)
            //{
            //    uint Damage = (uint)HitPoints;

            //    if (HitPoints > 0)
            //        HitPoints = 0;

            //    Game.MsgTournaments.MsgSchedules.ClanPhoenix.UpdateScore(stream, Damage, killer.Player.MyClan);
            //}
            //else if (UID == Game.MsgTournaments.MsgSchedules.ClanApe.Furnitures[StaticMesh.Pole].UID)
            //{
            //    uint Damage = (uint)HitPoints;

            //    if (HitPoints > 0)
            //        HitPoints = 0;

            //    Game.MsgTournaments.MsgSchedules.ClanApe.UpdateScore(stream, Damage, killer.Player.MyClan);
            //}
            //else if (UID == Game.MsgTournaments.MsgSchedules.ClanDesert.Furnitures[StaticMesh.Pole].UID)
            //{
            //    uint Damage = (uint)HitPoints;

            //    if (HitPoints > 0)
            //        HitPoints = 0;

            //    Game.MsgTournaments.MsgSchedules.ClanDesert.UpdateScore(stream, Damage, killer.Player.MyClan);
            //}
            //else if (UID == Game.MsgTournaments.MsgSchedules.ClanBird.Furnitures[StaticMesh.Pole].UID)
            //{
            //    uint Damage = (uint)HitPoints;

            //    if (HitPoints > 0)
            //        HitPoints = 0;

            //    Game.MsgTournaments.MsgSchedules.ClanBird.UpdateScore(stream, Damage, killer.Player.MyClan);
            //}


            else if (Map == 10620)
            {
                if (Mesh == StaticMesh.LeftGate)
                    Mesh = StaticMesh.OpenLeftGate;
                else if (Mesh == StaticMesh.RightGate)
                    Mesh = StaticMesh.OpenRightGate;
                else
                    Mesh = StaticMesh.OpenLeftGate;

                HitPoints = 0;
                Game.MsgServer.MsgUpdate upd = new Game.MsgServer.MsgUpdate(stream, UID, 2);
                stream = upd.Append(stream, Game.MsgServer.MsgUpdate.DataType.Mesh, (long)Mesh);
                stream = upd.Append(stream, Game.MsgServer.MsgUpdate.DataType.Hitpoints, (long)HitPoints);
                stream = upd.GetArray(stream);
                SendScrennPacket(stream);
            }
            else if (Game.MsgTournaments.MsgSchedules.GuildWar.Proces == Game.MsgTournaments.ProcesType.Alive
                // GW gates are handled above, not as a Large Monster
                && UID != Game.MsgTournaments.MsgSchedules.GuildWar.Furnitures[StaticMesh.LeftGate].UID
                && UID != Game.MsgTournaments.MsgSchedules.GuildWar.Furnitures[StaticMesh.RightGate].UID)
            {
                if (MaxHitPoints > 500000) // Threshold for Large Monsters/Bosses
                {
                    uint Damage = (uint)HitPoints;
                    if (HitPoints > 0) HitPoints = 0;
                    Game.MsgTournaments.MsgSchedules.GuildWar.UpdateScore(killer.Player, Damage);
                    Game.MsgTournaments.MsgSchedules.SendSysMesage(killer.Player.Name + " has defeated a Large Monster! Guild points updated.", MsgMessage.ChatMode.SlideCrosTheServer, MsgMessage.MsgColor.red);
                }
            }
            else if (HitPoints > 0)
            {
                HitPoints = 0;
            }
        }
        public unsafe void SendString(ServerSockets.Packet stream, Game.MsgServer.MsgStringPacket.StringID id, params string[] args)
        {
            Game.MsgServer.MsgStringPacket packet = new Game.MsgServer.MsgStringPacket();
            packet.ID = id;
            packet.UID = UID;
            packet.Strings = args;

            SendScrennPacket(stream.StringPacketCreate(packet));
        }
        public bool CanSee(IMapObj obj)
        {
            if (DynamicID != obj.DynamicID) return false;
            if (Map != obj.Map) return false;
            if (Core.GetDistance(X, Y, obj.X, obj.Y) >= 18)
                return false;
            return true;
        }
        public unsafe void SendView(ServerSockets.Packet msg, Role.GameMap GMap)
        {
            foreach (var user in GMap.View.Roles(MapObjectType.Player, X, Y, p => CanSee(p)))
            {
                user.Send(msg);
            }
        }
        public uint GuildID { get; set; }
        public unsafe void SendScrennPacket(ServerSockets.Packet packet)
        {
            foreach (var kv in Pool.GamePoll) // [perf] live lock-free enumeration (no .Values snapshot)
            {
                var client = kv.Value;
                if (client.Fake != false)
                    continue;
                if (client.Player.Map == Map)
                {
                    if (client.Player.GetMyDistance(X, Y) < SeedDistrance)
                    {
                        client.Send(packet);
                    }
                }
            }
        }
        public unsafe ServerSockets.Packet GetArray(ServerSockets.Packet stream, bool view)
        {
            if (statue != null)
            {
                if (statue.StatuePacket != null && statue.Static && statue.PacketVersion == Statue.CurrentPacketVersion)
                {
                    stream.Seek(0);
                    fixed (byte* ptr = statue.StatuePacket)
                    {
                        stream.memcpy(stream.Memory, ptr, statue.StatuePacket.Length);
                    }
                    stream.Size = statue.StatuePacket.Length;
                    return stream;
                }
                var champion = statue.user.Player;
                var guildName = champion.MyGuild != null ? champion.MyGuild.GuildName : string.Empty;
                var proto = new Role.Player.SpawnPacketProto()
                {
                    UID = UID,
                    Mesh = champion.Mesh,
                    Head = champion.HeadId,
                    Garment = champion.GarmentId,
                    LeftWeapon = champion.LeftWeaponId,
                    LeftWeaponSoul = champion.LeftWeapsonSoul,
                    RightWeapon = champion.RightWeaponId,
                    RightWeaponSoul = champion.RightWeapsonSoul,
                    LeftWeaponAccessory = champion.LeftWeaponAccessoryId,
                    RightWeaponAccessory = champion.RightWeaponAccessoryId,
                    Steed = champion.SteedId,
                    MountArmor = champion.MountArmorId,
                    AppearanceType = statue.Static ? Statue.DisplayAppearanceScale : (ushort)champion.AparenceType,
                    GuildRank = (uint)champion.GuildRank,
                    GuildID = champion.GuildID,
                    Wing = champion.WingTitleHaloHide ? 0 : champion.WingId,
                    WingPlus = champion.WingPlus,
                    WingProgress = champion.WingProgress,
                    Bottle = champion.Bottle,
                    Hitpoints = (uint)(HitPoints / 500),
                    X = X,
                    Y = Y,
                    HairStyle = champion.Hair,
                    Facing = (byte)champion.Angle,
                    Action = statue.Static ? (ushort)Role.Flags.ConquerAction.Sit : (ushort)statue.Action,
                    Reborn = champion.Reborn,
                    SecondRebornClass = champion.SecoundeClass,
                    FirstRebornClass = champion.FirstClass,
                    Level = champion.Level,
                    WindowSpawn = (byte)(statue.Static || view ? 1 : 0),
                    Away = champion.Away > 0,
                    ExtraBattlePower = champion.ExtraBattlePower,
                    FlowerIcon = (int)(champion.FlowerRank + 10000),
                    NobilityRank = (byte)champion.NobilityRank,
                    Armor = champion.ArmorId,
                    ArmorSoul = champion.ArmorSoul,
                    HeadSoul = champion.HeadSoul,
                    SteedColor = champion.SteedColor,
                    SteedPlus = champion.SteedPlus,
                    ClanUID = champion.ClanUID,
                    ClanRank = champion.ClanRank,
                    Title = champion.SomeHide ? 0 : (uint)champion.MyTitle,
                    ActiveSubClasses = (byte)champion.ActiveSublass,
                    SubClass = champion.SubClassHasPoints,
                    JiangHuActive = champion.JiangHuActive > 0,
                    JiangHuTalent = champion.JiangHuTalent,
                    MaxLife = champion.Owner.Status.MaxHitpoints,
                    RealUID = champion.RealUID,
                    ServerID = champion.ServerID,
                    UnionExploits = champion.KingDomExploits,
                    UnionID = champion.InUnion ? champion.MyUnion.UID : 0,
                    UnionRank = champion.InUnion ? (uint)Role.Instance.Union.Member.GetRank(champion.UnionMemeber.Rank) : 0,
                    UnionType = champion.InUnion ? (uint)champion.MyUnion.IsKingdom : 0,
                    BattlePower = (int)(champion.BattlePower - champion.GuildBattlePower),
                    QuizPoints = champion.QuizPoints,
                    Class = champion.Class,
                    GuildBattlePower = champion.GuildBattlePower,
                    MyWing = champion.WingTitleHaloHide ? 0 : champion.SpecialWingID,
                    MyTitle = champion.WingTitleHaloHide ? 0 : champion.SpecialTitleID,
                    MyTitleScore = champion.SpecialTitleScore,
                    CountryCode = champion.CountryID,
                    MainFlag = (byte)champion.MainFlag,
                    Official_Harem_Guards = champion.InUnion ? (uint)champion.ExploitsRank : 0,
                    FrameID = champion.FrameID,
                    HaloTitle = champion.WingTitleHaloHide ? 0 : champion.SpecialHaloID,
                    PrestigeRank = (byte)Database.PrestigeRanking.GetMyRank(Database.PrestigeRanking.GetIndex(champion.Class), champion.UID),
                    Name = System.Text.Encoding.UTF8.GetBytes(champion.Name ?? ""),
                    FamilyName = System.Text.Encoding.UTF8.GetBytes(""),
                    SyndicateName = System.Text.Encoding.UTF8.GetBytes(guildName ?? ""),
                    LeagueName = System.Text.Encoding.UTF8.GetBytes(champion.InUnion ? champion.MyUnion.Name : ""),
                    MateName = System.Text.Encoding.UTF8.GetBytes(champion.ClanName ?? ""),
                    OwnerPetName = System.Text.Encoding.UTF8.GetBytes(""),
                    Name134 = System.Text.Encoding.UTF8.GetBytes("")
                };
                proto.StatusFlags = new ulong[BitVector.bits.Length];
                stream.ProtoBufferSerialize(proto);
                stream.Finalize(Game.GamePackets.MsgPlayer);
                if (statue.StatuePacket == null && statue.Static)
                {
                    statue.StatuePacket = new byte[stream.Size];
                    int size = stream.Size;
                    fixed (byte* ptr = statue.StatuePacket)
                    {
                        stream.memcpy(ptr, stream.Memory, size);
                    }
                }
                return stream;
            }
            stream.InitWriter();
            var sobProto = new SobNpcSpawnProto() { UID = UID, Hitpoints = (uint)HitPoints, MaxHitpoints = (uint)MaxHitPoints, X = X, Y = Y, Mesh = (ushort)Mesh, Type = (byte)Type, Sort = Sort, Name = Name };
            stream.ProtoBufferSerialize(sobProto);
            stream.Finalize(Game.GamePackets.MsgNpcInfoEX);

            return stream;


        }
    }
}
