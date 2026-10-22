using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections.Concurrent;
using Qahr.Mobile.Game.MsgServer;
using Qahr.Mobile.Game.MsgNpc;
using Qahr.Mobile.Role.Instance;
using System.Windows.Forms;
using ProtoBuf;
using System.IO;
using Qahr.Mobile.Game.MsgTournaments;
using Qahr.Mobile.Game.MsgMonster;
using Qahr.Mobile.Database;
using Qahr.Mobile.Game.MsgFloorItem;

namespace Qahr.Mobile.Role
{
    public unsafe class Player : IMapObj
    {
        public DateTime ImpregnabilityEndTime;
        public uint ImpregnabilityValue;

        public DateTime BlackSpotEagle;

        public uint Medal = 0;

        [ProtoContract]
        public class SpawnPacketProto
        {
            [ProtoMember(1)]
            public uint UID;
            [ProtoMember(2)]
            public uint Mesh;
            [ProtoMember(3)]
            public ushort X;
            [ProtoMember(4)]
            public ushort Y;
            [ProtoMember(5)]
            public byte Facing;
            [ProtoMember(6)]
            public byte WindowSpawn;
            [ProtoMember(7)]
            public ushort Action;
            [ProtoMember(8)]
            public byte UsSubPositione;
            [ProtoMember(9)]
            public uint HairStyle;
            [ProtoMember(10)]
            public ulong[] StatusFlags = new ulong[18];
            [ProtoMember(11)]
            public uint GuildID;
            [ProtoMember(12)]
            public uint GuildRank;
            [ProtoMember(13)]
            public uint UsMemberTitle;
            [ProtoMember(14)]
            public ushort Level;
            [ProtoMember(15)]
            public bool Away;
            [ProtoMember(16)]
            public ushort AppearanceType;
            [ProtoMember(17)]
            public uint Head;
            [ProtoMember(18)]
            public uint Garment;
            [ProtoMember(19)]
            public uint Armor;
            [ProtoMember(20)]
            public uint LeftWeapon;
            [ProtoMember(21)]
            public uint RightWeapon;
            [ProtoMember(22)]
            public uint LeftWeaponAccessory;
            [ProtoMember(23)]
            public uint RightWeaponAccessory;
            [ProtoMember(24)]
            public uint Steed;
            [ProtoMember(25)]
            public uint MountArmor;
            [ProtoMember(26)]
            public uint Wing;
            [ProtoMember(27)]
            public uint WingPlus;
            [ProtoMember(28)]
            public uint WingProgress;
            [ProtoMember(29)]
            public uint Bottle;
            [ProtoMember(30)]
            public uint Hitpoints;//UsAntiMonster 
            [ProtoMember(31)]
            public uint SSpeedPercent;
            [ProtoMember(32)]
            public uint MaxLife;//DwLife
            [ProtoMember(33)]
            public uint DwMedalSelect;
            [ProtoMember(34)]
            public uint UsNoUsed;
            [ProtoMember(35)]
            public byte MonsterLevel;
            [ProtoMember(36)]
            public byte UcRev;
            [ProtoMember(37)]
            public byte Reborn;
            [ProtoMember(38)]
            public uint ExtraBattlePower;
            [ProtoMember(39)]
            public ushort DwFateBattleEffect;
            [ProtoMember(40)]
            public byte DwMemNumber;//TeamMembersCount
            [ProtoMember(41)]
            public uint DwLeaderID;//TeamLeaderId
            [ProtoMember(42)]
            public int FlowerIcon;
            [ProtoMember(43)]
            public byte NobilityRank;
            [ProtoMember(44)]
            public uint QuizPoints;
            [ProtoMember(45)]
            public uint SteedPlus;
            [ProtoMember(46)]
            public uint SteedExp;
            [ProtoMember(47)]
            public uint SteedColor;
            [ProtoMember(48)]
            public uint NVipLevel;
            [ProtoMember(49)]
            public uint ClanUID;
            [ProtoMember(50)]
            public ushort ClanRank;
            [ProtoMember(51)]
            public uint DwFamilyShareBattleEffect;
            [ProtoMember(52)]
            public uint Title;
            [ProtoMember(53)]
            public byte PokerSeat;
            [ProtoMember(54)]
            public uint PokerTableID;
            [ProtoMember(55)]
            public uint GuildBattlePower;
            [ProtoMember(56)]
            public byte InvisibleArena;
            [ProtoMember(57)]
            public bool RaceItem;
            [ProtoMember(58)]
            public byte BExploring;
            [ProtoMember(59)]
            public bool Boss;
            [ProtoMember(60)]
            public uint HeadSoul;
            [ProtoMember(61)]
            public uint ArmorSoul;
            [ProtoMember(62)]
            public uint LeftWeaponSoul;
            [ProtoMember(63)]
            public uint RightWeaponSoul;
            [ProtoMember(64)]
            public uint SubClass;
            [ProtoMember(65)]
            public ulong ActiveSubClasses; // [ProtoFix 2026-10-14] client CMsgPlayerPB #65 is uint64 (was byte)
            [ProtoMember(66)]
            public uint FirstRebornClass;
            [ProtoMember(67)]
            public uint SecondRebornClass;
            [ProtoMember(68)]
            public uint Class;
            [ProtoMember(69)]
            public ushort CountryCode;
            [ProtoMember(70)]
            public ushort Team;
            [ProtoMember(71)]
            public int BattlePower;
            [ProtoMember(72)]
            public byte JiangHuTalent;
            [ProtoMember(73)]
            public bool JiangHuActive;
            [ProtoMember(75)]
            public ushort ServerID;
            [ProtoMember(76)]
            public uint RealUID;
            [ProtoMember(77)]
            public uint OwnerPet;
            [ProtoMember(78)]
            public uint OwnerPet1;
            [ProtoMember(79)]
            public uint OwnerUID;
            [ProtoMember(80)]
            public uint UnionID;
            [ProtoMember(81)]
            public uint UnionExploits;
            [ProtoMember(82)]
            public uint Official_Harem_Guards;
            [ProtoMember(83)]
            public uint UnionRank;
            [ProtoMember(84)]
            public uint UnionType;
            [ProtoMember(85)]
            public uint MyTitle;
            [ProtoMember(86)]
            public uint MyTitleScore;
            [ProtoMember(87)]
            public uint MyWing;
            [ProtoMember(88)]
            public byte MainFlag;
            [ProtoMember(89)]
            public uint CloneCountFollow;
            [ProtoMember(90)]
            public byte Relic;
            [ProtoMember(92)]
            public long Member101;//5
            [ProtoMember(93)]
            public long Member102;//0
            [ProtoMember(94)]
            public bool UnknownBool;
            [ProtoMember(95)]
            public long[] HundredWeapons;
            [ProtoMember(96)]
            public byte PrestigeRank;
            [ProtoMember(97)]
            public uint MonstersID;
            [ProtoMember(98)]
            public byte SageMode1;
            [ProtoMember(99)]
            public uint SageMode2;
            [ProtoMember(101)]
            public uint ModelRGB;
            [ProtoMember(102)]
            public uint MountArmorColor;
            [ProtoMember(106)]
            public uint PistilAroma;
            [ProtoMember(108)]
            public uint HaloTitle;
            [ProtoMember(109)]
            public ushort FrameID;
            [ProtoMember(110)]
            public int todayFlowerType; // -1
            [ProtoMember(111)]
            public uint idFBLookFace; // -1
            [ProtoMember(112)]
            public ulong FlowerRank; // -1
            [ProtoMember(113)]
            public uint PandaPonpon; // -1
            [ProtoMember(119)]
            public uint Medal;
            [ProtoMember(125)]
            public uint IdentityFootprint;
            [ProtoMember(126)]
            public uint HaloActionID;
            [ProtoMember(131)]
            public byte[] Name;
            [ProtoMember(132)]
            public byte[] FamilyName;
            [ProtoMember(133)]
            public byte[] OwnerPetName;
            [ProtoMember(134)]
            public byte[] Name134;
            [ProtoMember(135)]
            public byte[] SyndicateName;
            [ProtoMember(136)]
            public byte[] LeagueName;
            [ProtoMember(137)]
            public byte[] MateName;
            // [ProtoFix 2026-10-14] client CMsgPlayerPB ends at #137: fields 138-144 do not exist in the 7957 client
            // (it skipped them as unknown). Wire serialization removed; the properties stay for server code.
            // Enlighten reaches the client via MsgUserAttrib (2046) DataType.EnlightPoints.
            // FootPrintID/ActionCool may belong to client #125/#126 (IdentityFootprint/HaloActionID) - needs evidence, not remapped.
            public uint InnerStrengthScore; // was [ProtoMember(138)] // [ProtoFix 2026-10-14]
            public uint Enlighten; // was [ProtoMember(139)] // [ProtoFix 2026-10-14]
            public uint FootPrintID; // was [ProtoMember(140)] // [ProtoFix 2026-10-14]
            public uint ActionCool; // was [ProtoMember(141)] // [ProtoFix 2026-10-14]
            public uint FootPrintScore; // was [ProtoMember(142)] // [ProtoFix 2026-10-14]
            public uint ActionCoolScore; // was [ProtoMember(143)] // [ProtoFix 2026-10-14]
            public uint MachineInfo; // was [ProtoMember(144, IsRequired = true)] // [ProtoFix 2026-10-14]
        }
        #region New System
        public uint RegularCallCount;
        public uint PreciousCallingCount;
        public uint EonspiritLevel = 0;
        public uint EonspiritCurrentEnergy = 0;
        public uint EonspiritPrestrige;
        public uint AmazingSpeedLayer;
        public uint SwordBodyMax;
        public uint SwordBodyPercentage;
        public uint BenefitShowerHP;
        public bool UnLockedSystem = false;
        #endregion
        public string Name
        {
            get
                ;
            set
                ;
        }
        public List<MonsterRole> ListPets = new List<MonsterRole>();
        public DaysNobility PayNobilitySystem;
        public bool FullLoadings = false;
        public bool MapGuildWar;
        public uint KillCount = 0;
        public byte KillCountBCPs = 0;
        #region InfoArchives
        public uint EnergyPoints = 0;

        public uint EnergyPointsTalent = 0;

        public uint EnergyPointsAntiwar = 0;

        public uint EnergyPointsAntifatalism = 0;

        public uint EnergyPointsMystic = 0;


        public byte stage = 0;

        public byte Laststage = 0;

        public byte CountMonster = 0;

        public byte CountMonster2 = 0;

        public bool Stage = false;

        public byte FinishStage1 = 0;
        public byte FinishStage2 = 0;

        public byte Portal = 0;

        public byte TaoArchives = 0;

        public uint StarChain = 0;

        public byte PirateArchives = 0;

        public byte HaveArchives = 0;

        public uint WeepStronCantidad = 0;

        public byte HuaMulan = 0;

        public byte Archives = 0;

        public byte Ares = 0;

        public byte MonsterTyre = 0;

        public byte Tyre = 0;

        public uint CrackMantra1 = 0;

        public uint CrackMantra2 = 0;

        public int ArrowBladesPower = 0;
        public int PengchengMilesCount = 0;
        public DateTime PengchengMilesStamp = new DateTime();
        public DateTime EquipStamp = new DateTime();
        public uint PDefence = 0;

        public uint MDefence = 0;


        public uint GrowFromHurtHitpoints = 0;
        public Time32 GrowFromHurtStamp;


        public void UpdateGrowFromHurtHitpoints(ServerSockets.Packet stream, uint HitPoints = 0)
        {

        }


        public void UpdateArrowBlades(ServerSockets.Packet stream, int Power = 0)
        {
            if (Power < 0 && ArrowBladesPower == 0)
                return;
            ArrowBladesPower += Power;
            ArrowBladesPower = Math.Min(3, ArrowBladesPower);
            if (!Database.AtributesStatus.IsArcher(Class))
                return;
            if (!ContainFlag(MsgUpdate.Flags.ActiveArrowBlades))
                AddSpellFlag(Game.MsgServer.MsgUpdate.Flags.ActiveArrowBlades, Role.StatusFlagsBigVector32.PermanentFlag, true);
            SendUpdate(stream, Game.MsgServer.MsgUpdate.Flags.ActiveArrowBlades, 0, (uint)ArrowBladesPower, 0, MsgUpdate.DataType.AppendIcon, true);
            if (ContainFlag(MsgUpdate.Flags.ActiveArrowBlades) && ArrowBladesPower == 0)
                RemoveFlag(MsgUpdate.Flags.ActiveArrowBlades);
            Send(stream);

        }
        public void UpdatePengchengMiles(ServerSockets.Packet stream, int Power = 0)
        {
            if (Power < 0 && PengchengMilesCount == 0)
                return;
            PengchengMilesCount += Power;
            PengchengMilesCount = Math.Min(3, PengchengMilesCount);
            if (!Database.AtributesStatus.IsLee(Class) && !Owner.MySpells.ClientSpells.ContainsKey((ushort)Role.Flags.SpellID.KunpengTrek))
                return;
            SendUpdate(stream, (MsgUpdate.Flags)UID, 8, (uint)PengchengMilesCount, (uint)3, (MsgUpdate.DataType)143, true);
            Send(stream);
        }
        #endregion

        public int DuneEnergy = 0;
        public int DuneEnergy2 = 0;
        public int DuneEnergy3 = 0;
        public DateTime DuneStampPower = DateTime.Now;

        //#region TwistedFututr
        //public uint TwistedFututrOpenRank = 0;
        //public uint ApePistolSkill = 0;

        //public uint TwistedFututrExp = 0;
        //public uint TwistedFututrLevel = 0;
        //public uint TwistedFututrAirPower = 0;
        //public uint TwistedFututrLevelSkill = 0;
        //public uint TwistedFututrUnlocked = 0;//bool
        //public uint TwistedFututrSelectMaterial = 0;
        //#endregion

        public DateTime LastMove;
        private byte _NiniaP0;
        public DateTime YuanshenStamp = DateTime.Now;

        public byte BlueRune;
        public byte RedRunes;

        public byte NiniaP0
        {
            get
            {
                return _NiniaP0;
            }
            set
            {
                _NiniaP0 = value;
                using (var rec = new ServerSockets.RecycledPacket())
                {
                    var stream = rec.GetStream();
                    var packet = new MsgUpdate(stream, UID, 1);
                    stream = packet.Append(stream, MsgUpdate.DataType.SageModeLevel, _NiniaP0);
                    stream = packet.GetArray(stream);
                    Owner.Send(stream);
                }
            }
        }
        private uint _MyDontion;
        public uint MyDontion
        {
            get
            {
                return _MyDontion;
            }
            set
            {
                _MyDontion = value;
                using (var rec = new ServerSockets.RecycledPacket())
                {
                    var stream = rec.GetStream();

                    SendUpdate(stream, (long)value, MsgUpdate.DataType.MyDontion);
                    SendUpdate(stream, (long)value, MsgUpdate.DataType.DonationPoints);
                    Owner.Send(stream);
                }

            }
        }
        private uint _CyanJadeRing;
        public uint CyanJadeRing
        {
            get
            {
                return _CyanJadeRing;
            }
            set
            {
                _CyanJadeRing = value;
                using (var rec = new ServerSockets.RecycledPacket())
                {
                    var stream = rec.GetStream();

                    SendUpdate(stream, (long)value, MsgUpdate.DataType.CyanJadeRing);
                    Owner.Send(stream);
                }

            }
        }
        public int TodayFlowerType
        {
            get
            {
                if (Role.Core.IsGirl(Mesh % 10000))
                {
                    if (Flowers != null)
                    {

                        if (Flowers.RedRoses.Amount2day > Flowers.Lilies.Amount2day && Flowers.RedRoses.Amount2day > Flowers.Orchids.Amount2day &&
                            Flowers.RedRoses.Amount2day > Flowers.Tulips.Amount2day)
                            return (int)Math.Min(Flowers.RedRoses.Amount2day, 999);

                        else if (Flowers.Lilies.Amount2day > Flowers.Orchids.Amount2day &&
                             Flowers.Lilies.Amount2day > Flowers.Tulips.Amount2day)
                            return (int)(1000 + Math.Min(Flowers.Lilies.Amount2day, 999));

                        else if (Flowers.Orchids.Amount2day > Flowers.Tulips.Amount2day)
                            return (int)(2000 + Math.Min(Flowers.Orchids.Amount2day, 999));

                        else if (Flowers.Tulips.Amount2day > 0)
                            return (int)(3000 + Math.Min(Flowers.Orchids.Amount2day, 999));

                        else if (Flowers.RedRoses.FlowerFree > 0)
                            return 1;
                    }

                }
                else
                {
                    if (Flowers != null)
                    {
                        if (Flowers.RedRoses.Amount2day > Flowers.Lilies.Amount2day &&
                              Flowers.RedRoses.Amount2day > Flowers.Orchids.Amount2day &&
                               Flowers.RedRoses.Amount2day > Flowers.Tulips.Amount2day)
                            return (int)(4000 + Math.Min(Flowers.RedRoses.Amount2day, 999));

                        else if (Flowers.Lilies.Amount2day > Flowers.Orchids.Amount2day &&
                             Flowers.Lilies.Amount2day > Flowers.Tulips.Amount2day)
                            return (int)(5000 + Math.Min(Flowers.Lilies.Amount2day, 999));

                        else if (Flowers.Orchids.Amount2day > Flowers.Tulips.Amount2day)
                            return (int)(6000 + Math.Min(Flowers.Orchids.Amount2day, 999));

                        else if (Flowers.Tulips.Amount2day > 0)
                            return (int)(7000 + Math.Min(Flowers.Tulips.Amount2day, 999));
                    }
                }
                return -1000;
            }

        }

        public int VenomDamage = 0;
        public uint BonePulse, PaperDance;
        public uint DeityLandLuckyPoints;
        public uint HeroPoints;
        public byte CPBoundPack = 0;
        public DateTime LeftDoorStamp, RightDoorStamp;
        public bool CanUseSectorSkill = false;
        public bool ClaimedBCPToday = false;
        public string PINCODE = "", OrginalInput = "";
        public string PinCodeAnima = "", OrginalInput2 = "";
        private uint tsEnergy, tsUndyingImprinting;
        public uint ThundercloudSight;
        public uint ThunderStrikerEnergy
        {
            get
            {
                return tsEnergy;
            }
            set
            {
                tsEnergy = value;

                if (value >= 100)
                    AddFlag(MsgUpdate.Flags.DevouringStrike, Role.StatusFlagsBigVector32.PermanentFlag, false);
                else
                    RemoveFlag(MsgUpdate.Flags.DevouringStrike);
                using (var recycledPacket = new ServerSockets.RecycledPacket())
                {
                    var streamm = recycledPacket.GetStream();
                    {
                        SendUpdate(streamm, MsgUpdate.Flags.DevouringStrike, 0, value, 0, MsgUpdate.DataType.DevouringStrike);
                    }
                }
            }
        }
        public uint ThunderStrikerUndyingImprinting
        {
            get
            {
                return tsUndyingImprinting;
            }
            set
            {
                tsUndyingImprinting = value;
                if (value >= 60)
                {
                    AddFlag(MsgUpdate.Flags.UndyingImprinting, Role.StatusFlagsBigVector32.PermanentFlag, false);
                }
                else RemoveFlag(MsgUpdate.Flags.UndyingImprinting);
                if (value % 60 == 0)
                {
                    using (var recycledPacket = new ServerSockets.RecycledPacket())
                    {
                        var streamm = recycledPacket.GetStream();
                        {
                            SendUpdate(streamm, MsgUpdate.Flags.UndyingImprinting, 60 - (value % 60), (uint)(value / 60), 0, MsgUpdate.DataType.UndyingImprinting);
                        }
                    }
                }
            }
        }
        public uint WildPhoenixDamage = 0;
        public uint SuanniCommandCount = 0;
        public int SupremeLeadershipCount = 0;
        public uint HoverFatherDamage = 0;
        #region SparkShield
        public uint SparkShieldDamage = 0;
        public byte SparkShieldLevel;
        public int SparkShieldTime = 0;
        public DateTime SparkShieldStamp;
        public void SendSparkShield(ServerSockets.Packet stream)
        {
            var Time = SparkShieldStamp.AddSeconds(SparkShieldTime) - DateTime.Now;
            SendUpdate(stream, MsgUpdate.Flags.SparkShieldActivated, (uint)Time.Seconds, (uint)SparkShieldDamage, SparkShieldLevel, MsgUpdate.DataType.AzureShield, true);
        }
        #endregion
        #region IronShield
        public byte IronShieldLevel;
        public uint IronShieldDamage;
        public int IronShieldTime = 0;
        public DateTime IronShieldStamp;
        public void SendIronShield(ServerSockets.Packet stream)
        {
            var Time = IronShieldStamp.AddSeconds(IronShieldTime) - DateTime.Now;
            SendUpdate(stream, MsgUpdate.Flags.IronShield, (uint)Time.Seconds, (uint)IronShieldDamage, IronShieldLevel, MsgUpdate.DataType.AzureShield, true);
        }
        #endregion
        public byte DragonSwingChance;
        public int MiracleTouch;
        public int SlayerPercent;
        public byte CountItem;
        public byte ImmortalForceTime;
        public DateTime SlayerStamp;
        public Time32 UndyingWillStamp;
        public uint CleanSweepPower, SongofPhoenixPower, HawksAmbitionPower, HookMoonPower, HookMoonAttackedPower, DeadlyStrikePower, DeathSighPassive, DeathSighActive, AxeShadowPower, CelestialDancePower, RoamerPower;
        public Time32 VigourStamp, WhettedBladeStamp, AbsolutionStamp, RampageStamp, BloodTideStamp, HealerStamp, DuelEndStamp, TidalWaveStamp, QuenchStamp, LightningShieldStamp;
        public uint FineRainHPlose, FineRainHP, FineRainPower, FuryStrikeHP, BloodTideCaptured, defFineRainPower, BloodTideHP, RiseofTaoismExtraMDamage, InfinitySDamage, InfinityDamage, LightningShieldLeft, RiseofTaoismHP;
        public bool FineRainHit = false;
        public uint InsouciancePower, ImmersionPower;
        public Player CrackStarNegativeDealer;
        public byte FuryStrikeAttack, DuelPercent, XPBooster = 0, XPKiller = 0;
        public uint DuelUID = 0;
        public Time32 SickleWindStamp;
        public void IncreaseXPDuration(short seconds)
        {
            if (BitVector.ArrayFlags.ContainsKey((int)OnXPSkill()))
                BitVector.ArrayFlags[(int)OnXPSkill()].Secounds += seconds;
        }
        public bool RelicSpirit;
        public bool TREPIN = false, MsgTrue = false;
        public bool TREPIN2 = false;
        public bool AnimaLockedTimer = false;
        public bool CheckPin()
        {
            return PinCodeAnima != "" ? true : false;
        }
        public bool AttackHit = false;
        public bool HitInMele = false;
        public bool IsStillBanned { get { return (this.BannedChatStamp >= DateTime.Now && this.IsBannedChat) || this.PermenantBannedChat; } }
        public DateTime BannedChatStamp;
        public bool PermenantBannedChat;
        public bool IsBannedChat;
        public Client.GameClient AttackerScarofEarthl;
        public InteractQuery AttackPacket;
        public Database.MagicType.Magic ScarofEarthl;
        public Action<Player> OnDeath;
        public DateTime UseLayTrap = new DateTime();

        public DateTime ItemsPeriodStamp = new DateTime();
        public DateTime EarthStamp = new DateTime();

        public DateTime CanChangeWindWalkerFree = DateTime.Now;
        public DateTime FanRecoverStamin = new DateTime();


        public uint Bet = 0;
        public uint RivalUID = 0;



        public int RevengeTailChange = 0;

        public byte UseChiToken = 0;
        public ushort ClaimPointsArena = 0;
        public bool OnRemoveLukyAmulet = false;
        public bool OnBluedBird = false;
        public DateTime BlueBirdPlumeStamp = new DateTime();

        public bool OnFerentPill { get { return ContainFlag(MsgUpdate.Flags.Poisoned); } }
        public DateTime FerventPill = new DateTime();

        public byte LuiseQuestions = 0;
        public DateTime RealPortraitStamp = new DateTime();
        public DateTime RevealVialStamp = new DateTime();

        public bool IsBoy()
        {
            return Role.Core.IsBoy(Body);
        }
        public bool IsGirl()
        {
            return Role.Core.IsGirl(Body);
        }
        public int GiveFlowersToPerformer = 0;

        public DateTime GallbladerrStamp = new DateTime();

        public byte MonkMiseryTransforming = 0;

        public uint[] EpicTrojanItemChange = new uint[2];
        public byte StageEpicTrojanQuest = 0;
        public uint ChangeEpicTrojan = 0;
        public byte CanChangeEpicMaterial = 1;

        public uint EpicTrojanEvilArrayPoints = 0;
        public uint EpicTrojanAbysalStage = 0;

        public uint ChangeArrayEpicTrojan = 0;
        public byte CanChangeArrayEpicMaterial = 1;
        public uint[] EpicTrojanArrayItemChange = new uint[2];

        public byte EpicTrijanKillGhostReaver = 0;
        public uint[] EpicTrojanMr_MirrorItemChange = new uint[2];
        public uint ChangeMr_MirrorEpicTrojan = 0;
        public byte CanChangeMr_MirrorEpicMaterial = 1;


        public uint[] EpicTrojanGeneralPakItemChange = new uint[2];
        public uint ChangeGeneralPakEpicTrojan = 0;
        public byte CanChangGeneralPakMaterial = 1;

        public byte EpicTrojanMrMirrorPrograss = 0;

        public Int32 SelectedStage, SelectedAttribute;


        public void ResetEpicTrojan()
        {
            EpicTrojanMrMirrorPrograss = 0;
            StageEpicTrojanQuest = 0;
            EpicTrojanEvilArrayPoints = 0;
            EpicTrojanAbysalStage = 0;
            EpicTrijanKillGhostReaver = 0;
            ChangeEpicTrojan = ChangeArrayEpicTrojan = ChangeMr_MirrorEpicTrojan = ChangeGeneralPakEpicTrojan = 0;
            CanChangeEpicMaterial = CanChangeArrayEpicMaterial = CanChangeMr_MirrorEpicMaterial = CanChangGeneralPakMaterial = 1;
        }
        public string SaveEpicTrojan()
        {
            Database.DBActions.WriteLine writer = new Database.DBActions.WriteLine('/');
            writer.Add(EpicTrojanItemChange[0]).Add(EpicTrojanItemChange[1]).Add(StageEpicTrojanQuest)
                .Add(ChangeEpicTrojan).Add(CanChangeEpicMaterial).Add(CanChangeArrayEpicMaterial)
                .Add(ChangeArrayEpicTrojan).Add(EpicTrojanArrayItemChange[0]).Add(EpicTrojanArrayItemChange[1])
                .Add(EpicTrojanEvilArrayPoints).Add(EpicTrojanAbysalStage).Add(EpicTrojanMr_MirrorItemChange[0])
                .Add(EpicTrojanMr_MirrorItemChange[1]).Add(ChangeMr_MirrorEpicTrojan)
                .Add(CanChangeMr_MirrorEpicMaterial)

                .Add(EpicTrojanGeneralPakItemChange[0]).Add(EpicTrojanGeneralPakItemChange[1])
                .Add(ChangeGeneralPakEpicTrojan).Add(CanChangGeneralPakMaterial)
                .Add(EpicTrojanMrMirrorPrograss);
            return writer.Close();
        }

        public void LoadEpicTrojan(string line)
        {
            if (line == null)
                return;
            Database.DBActions.ReadLine reader = new Database.DBActions.ReadLine(line, '/');
            EpicTrojanItemChange[0] = reader.Read((uint)0);
            EpicTrojanItemChange[1] = reader.Read((uint)0);
            StageEpicTrojanQuest = reader.Read((byte)0);
            ChangeEpicTrojan = reader.Read((uint)0);
            CanChangeEpicMaterial = reader.Read((byte)0);
            CanChangeArrayEpicMaterial = reader.Read((byte)0);
            ChangeArrayEpicTrojan = reader.Read((uint)0);
            EpicTrojanArrayItemChange[0] = reader.Read((uint)0);
            EpicTrojanArrayItemChange[1] = reader.Read((uint)0);
            EpicTrojanEvilArrayPoints = reader.Read((uint)0);
            EpicTrojanAbysalStage = reader.Read((uint)0);
            EpicTrojanMr_MirrorItemChange[0] = reader.Read((uint)0);
            EpicTrojanMr_MirrorItemChange[1] = reader.Read((uint)0);
            ChangeMr_MirrorEpicTrojan = reader.Read((uint)0);
            CanChangeMr_MirrorEpicMaterial = reader.Read((byte)0);

            EpicTrojanGeneralPakItemChange[0] = reader.Read((uint)0);
            EpicTrojanGeneralPakItemChange[1] = reader.Read((uint)0);
            ChangeGeneralPakEpicTrojan = reader.Read((uint)0);
            CanChangGeneralPakMaterial = reader.Read((byte)0);
            EpicTrojanMrMirrorPrograss = reader.Read((byte)0);
        }
        public string SaveEpicArcher()
        {
            Database.DBActions.WriteLine writer = new Database.DBActions.WriteLine('/');
            writer.Add(EnergyPoints).Add(EnergyPointsTalent).Add(EnergyPointsAntiwar).Add(EnergyPointsAntifatalism).Add(EnergyPointsMystic).Add(stage).Add(Laststage).Add(FinishStage1).Add(HaveArchives).Add(TaoArchives).Add(PirateArchives);


            return writer.Close();
        }

        public void LoadEpicArcher(string line)
        {
            if (line == null)
                return;
            Database.DBActions.ReadLine reader = new Database.DBActions.ReadLine(line, '/');
            EnergyPoints = reader.Read((uint)0);
            EnergyPointsTalent = reader.Read((uint)0);
            EnergyPointsAntiwar = reader.Read((uint)0);
            EnergyPointsAntifatalism = reader.Read((uint)0);
            EnergyPointsMystic = reader.Read((uint)0);
            stage = reader.Read((byte)0);
            Laststage = reader.Read((byte)0);
            FinishStage1 = reader.Read((byte)0);
            HaveArchives = reader.Read((byte)0);
            TaoArchives = reader.Read((byte)0);
            PirateArchives = reader.Read((byte)0);
        }
        public string SaveArchiveWarrior()
        {
            Database.DBActions.WriteLine writer = new Database.DBActions.WriteLine('/');
            writer.Add(HuaMulan).Add(Ares).Add(Tyre);


            return writer.Close();
        }
        public void LoadArchiveWarrior(string line)
        {
            if (line == null)
                return;
            Database.DBActions.ReadLine reader = new Database.DBActions.ReadLine(line, '/');
            HuaMulan = reader.Read((byte)0);
            Ares = reader.Read((byte)0);
            Tyre = reader.Read((byte)0);

        }
        public int DefeatedArenaGuardians = 0;

        public DateTime JoinPowerArenaStamp = new DateTime();


        public DateTime JoinPrizeNpcOctopus = new DateTime();

        public bool TOM_StartChallenge = false;
        public bool TOM_FinishChallenge = false;

        public byte ClaimTowerAmulets = 0;
        public DateTime TowerAmuletStamp = new DateTime();


        public byte TOMClaimTeamReward = 0;
        public DateTime TowerOfMysteryFrezeeStamp = new DateTime();
        public byte JoinTowerOfMysteryLayer = 0;
        public byte MyTowerOfMysteryLayer = 0;
        public byte MyTowerOfMysteryLayerElite = 0;
        public byte TowerOfMysterychallenge = 3;
        public bool CanSweapTowerOfMystery = false;

        public uint TowerOfMysteryChallengeFlag = 0;
        public byte TOMSelectChallengeToday = 0;
        public byte TOMChallengeToday = 0;
        public uint TOMRefreshReward = 0;
        public uint ItemRewordChristmas = 0;
        public Game.MsgTournaments.MsgTowerOfMystery.RewardTypes TOM_Reward = Game.MsgTournaments.MsgTowerOfMystery.RewardTypes.Normal;

        public byte TOM_SellectOptionC = 0;
        public uint CollectionID = 0;
        public uint PandaID = 0;
        public byte OpenHousePack = 0;
        public bool HideFloor = false;
        public bool WingTitleHaloHide = false;
        public bool SomeHide = false;
        public bool SigilNinja = false;
        public bool SigilPirate = false;
        public byte GiftBackUp = 0;

        public void InitializeTransfer(uint ServerID)
        {
            if (ConquerPoints > 1)
            {
                var server = Database.GroupServerList.GetServer(ServerID);
                TransferToServer = server.Name;
                CheckTransfer = true;
                MsgInterServer.PipeClient.Connect(Owner, server.IPAddress, server.Port);
                Owner.CreateBoxDialog("We're preparing your transfer , please stand by ...");
            }
            else
                Owner.CreateBoxDialog("You need to have 1 CPs for transfer ! ");
        }
        public string TransferToServer = "";

        public uint InitTransfer = 0;
        public bool CheckTransfer = false;
        public bool OnTransfer = false;
        private ushort extraAtr;

        public ushort ExtraAtributes
        {
            get { return extraAtr; }
            set
            {
                if (value > 903)
                    value = 903;

                extraAtr = value;
            }
        }
        public void AddExtraAtributes(ServerSockets.Packet stream, ushort value)
        {
            if (ExtraAtributes + value > 903)
                value = (ushort)(538 - ExtraAtributes);

            ExtraAtributes += value;
            Atributes += value;

            SendUpdate(stream, Atributes, MsgUpdate.DataType.Atributes);
        }
        public enum RechargeType : ulong
        {
            None = 0,
            OneGarment = 1u << 0,
            TwoVipTokens = 1u << 1,
            OneMount = 1u << 2,
            InnerPower1000 = 1u << 3,
            FourVipTokens = 1u << 4,
            ConquerPoints225 = 1u << 5,
            InnerPower1500 = 1u << 6,
            InnerPower5000 = 1u << 7,
            OneMount2 = 1u << 8,
            OneRareAccesory = 1u << 9,
            OneGerment2 = 1u << 10,
            InnerPower5000_2 = 1u << 11,
            ConquerPoints500 = 1u << 12,
            OneSoulP7 = 1u << 13,
            OneRareMaterial = 1u << 14,
            Vip6Tokens = 1u << 15,
            OneRareAccesory_2 = 1u << 16,
            InnerPower5000_3 = 1u << 17,
            OneMount3 = 1u << 18,
            GoldPrize = 1u << 19,
            InnerPower10000 = 1u << 20
        }
        public RechargeType RechargeProgress = RechargeType.None;
        public bool ContainRechargeType(RechargeType flag)
        {
            return (RechargeProgress & flag) == flag;
        }
        public bool AddRechargeFlag(RechargeType flag)
        {
            if (!ContainRechargeType(flag))
            {
                RechargeProgress |= flag;

                Database.RechargeShop.UpdateRecharge(Owner);

                return true;
            }
            return false;
        }
        public uint RechargePoints = 0;

        public byte BuyKingdomDeeds = 0;
        public uint KingDomDeeds = 0;


        public void UpdateKingdomTreasury(uint points)
        {
            if (InUnion)
            {
                MyUnion.Treasury += points;
                UnionMemeber.MyTreasury += points;
            }
        }

        public bool OnMyOwnServer
        {
            get { return ServerID == Database.GroupServerList.MyServerInfo.ID; }
        }

        public ushort ServerID = 0;
        public ushort SetLocationType = 0;
        public bool InUnion
        {
            get { return MyUnion != null && UnionMemeber != null; }
        }
        public bool IsUnionEmperror
        {
            get { return MyUnion != null && UnionMemeber != null && UnionMemeber.Rank == Instance.Union.Member.MilitaryRanks.Emperor; }
        }
        public Role.Instance.Union MyUnion = null;
        public Role.Instance.Union.Member UnionMemeber = null;

        public DateTime SickleStamp2 = new DateTime();

        public int lastClientJumpTime;
        public DateTime PreviousJump, PreviousMove;
        public DateTime JumpingStamp = new DateTime();
        public DateTime JumpingPrevious = new DateTime();
        public bool OnSpeedFlags()
        {
            return ContainFlag(MsgUpdate.Flags.Cyclone) || ContainFlag(MsgUpdate.Flags.Oblivion) || ContainFlag(MsgUpdate.Flags.AmazingSpeed) || ContainFlag(MsgUpdate.Flags.CannonBarrage) ||
                ContainFlag(MsgUpdate.Flags.SuperCyclone) || ContainFlag(MsgUpdate.Flags.ScarofEarth) || ContainFlag(MsgUpdate.Flags.Omnipotence) || ContainFlag(MsgUpdate.Flags.ManiacDance) || ContainFlag(MsgUpdate.Flags.DragonCyclone) ||
                ContainFlag(MsgUpdate.Flags.BladeFlurry) ||
                ContainFlag(MsgUpdate.Flags.FatalStrike);
        }
        public int CountJumpHack;
        public int CountMoveHack;
        public DateTime MoveingStamp = new DateTime();
        public DateTime MovePrevious = new DateTime();
        public DateTime StunJump = DateTime.Now;
        public Time32 lastWalkTime = Time32.Now;
        public DateTime ManiacDanceStamp = new DateTime();
        public int CountFallHack;
        public DateTime KingOfTheHillStamp = new DateTime();
        public uint KingOfTheHillScore = 0;
        public uint SkillTournamentLifes = 0;

        public const ushort MaxInventorySashCount = 300;
        public ushort InventorySashCount = 0;
        public bool Invisible = false;
        public void UpdateInventorySash(ServerSockets.Packet stream)
        {
            SendUpdate(stream, MaxInventorySashCount, MsgUpdate.DataType.InventorySashMax);
            SendUpdate(stream, InventorySashCount, MsgUpdate.DataType.InventorySash);
        }
        public DateTime StampSecorSpells = new DateTime();
        public DateTime StampBloodyScytle = new DateTime();
        public DateTime MedicineStamp = new DateTime();
        public int ReceiveTestT = 0;
        public DateTime ReceiveTest;
        public DateTime ReceivePing = new DateTime();



        public uint AtiveQuestApe = 0;

        public uint QuestCaptureType = 0;
        public Random MyRandom = new Random(Pool.GetRandom.Next());
        public bool Rate(int value)
        {
            return value > MyRandom.Next() % 100;
        }
        public DateTime SickleStamp = new DateTime();
        public int FootballTeamID = 0;
        public bool OnAutoHunt = false;
        public ulong AutoHuntExp = 0;
        public uint FootBallMatchPoints = 0;
        public uint MyFootBallPoints = 0;

        public ulong DailySignUpDays = 0;
        public byte DailySignUpRewards = 0;
        public byte DailyMonth = 0;
        public uint DailyDays
        {
            get
            {
                uint days = 0;
                for (byte x = 0; x < 31; x++)
                    if ((DailySignUpDays & (1ul << x)) == (1ul << x))
                        days += 1;
                return days;
            }
        }

        public uint TaskReward = 0;
        public uint TaskRewardIndex = 0;
        public uint CountSpeedHack = 0;
        public List<byte> Times10 = new List<byte>();
        public ushort CountryID = 0;


        public void SwitchWingWalkerAttack(ServerSockets.Packet stream)
        {
            if ((MainFlag & MainFlagType.OnMeleeAttack) == MainFlagType.OnMeleeAttack)
            {
                MainFlag &= ~MainFlagType.OnMeleeAttack;


                SendUpdate(stream, (uint)MainFlag, MsgUpdate.DataType.MainFlag);
                Owner.CreateBoxDialog("You have successfully changed your branch to Chaser(Ranged).");


            }
            else
            {
                MainFlag |= MainFlagType.OnMeleeAttack;
                SendUpdate(stream, (uint)MainFlag, MsgUpdate.DataType.MainFlag);
                Owner.CreateBoxDialog("You have successfully changed your branch to Stomper(Melee).");
            }
        }
        public enum MainFlagType : uint
        {
            None = 0,
            CanClaim = 1 << 0,
            ShowSpecialItems = 1 << 1,
            ClaimGift = 1 << 2,
            OnMeleeAttack = 1 << 3,
        }
        public MainFlagType MainFlag = 0;
        public ushort NameEditCount = 0;
        public uint CurrentTreasureBoxes = 0;
        public DateTime TaskQuestTimer = new DateTime();
        public uint QuestMultiple = 0;
        public class WardRobeTitle
        {
            public uint titleID;
            public ulong TotalSeconds;
            public DateTime DateStamp;
        }
        public DateTime AzurePillStamp = new DateTime();
        public ConcurrentDictionary<uint, WardRobeTitle> TitleWithTime = new ConcurrentDictionary<uint, WardRobeTitle>();
        public List<MsgTitleStorage.TitleType> SpecialTitles = new List<MsgTitleStorage.TitleType>();
        public List<MsgTitleStorage.Footprint> Footprint = new List<MsgTitleStorage.Footprint>();
        public List<MsgTitleStorage.HaloAction> HaloAction = new List<MsgTitleStorage.HaloAction>();
        public List<MsgTitleStorage.HaloType> HaloTitles = new List<MsgTitleStorage.HaloType>();
        public List<MsgTitleStorage.KillEffects> KillEffectsAction = new List<MsgTitleStorage.KillEffects>();
        public List<MsgTitleStorage.WingsType> WingsTitles = new List<MsgTitleStorage.WingsType>();
        public uint GetTitlesScore()
        {
            uint score = 0;
            foreach (var title in SpecialTitles)
            {
                if (Database.TitleStorage.Titles.ContainsKey((uint)title))
                    score += Database.TitleStorage.Titles[(uint)title].Score;
            }
            return score;
        }
        public uint GetHaloScore()
        {
            uint score = 0;
            foreach (var title in HaloTitles)
            {
                if (Database.TitleStorage.Titles.ContainsKey((uint)title))
                    score += Database.TitleStorage.Titles[(uint)title].Score;
            }
            return score;
        }
        public uint GetWingsScore()
        {
            uint score = 0;
            foreach (var title in HaloTitles)
            {
                if (Database.TitleStorage.Titles.ContainsKey((uint)title))
                    score += Database.TitleStorage.Titles[(uint)title].Score;
            }
            return score;
        }
        public uint GetFootprintScore()
        {
            uint score = 0;
            foreach (var title in Footprint)
            {
                if (Database.TitleStorage.Titles.ContainsKey((uint)title))
                    score += Database.TitleStorage.Titles[(uint)title].Score;
            }
            return score;
        }
        public uint GetHaloActionScore()
        {
            uint score = 0;
            foreach (var title in HaloAction)
            {
                if (Database.TitleStorage.Titles.ContainsKey((uint)title))
                    score += Database.TitleStorage.Titles[(uint)title].Score;
            }
            return score;
        }
        public void AddTimeTitle(uint TitleID, ServerSockets.Packet stream)
        {
            Database.TitleStorage dbtitle;
            if (Database.TitleStorage.Titles.TryGetValue((uint)TitleID, out dbtitle))
            {
                WardRobeTitle Time;
                if (TitleWithTime.TryGetValue((uint)TitleID, out Time))
                {
                    MsgTitleStorage.TitleStorage title = new MsgTitleStorage.TitleStorage();
                    title.ActionID = MsgTitleStorage.Action.UpdateScore;
                    title.dwparam1 = GetTitlesScore();
                    title.dwparam2 = dbtitle.ID;
                    title.dwparam3 = dbtitle.SubID;
                    title.Title = new MsgTitleStorage.Title();
                    title.Title.ID = dbtitle.ID;
                    title.Title.SubId = dbtitle.SubID;
                    TimeSpan timeSpan = Time.DateStamp - DateTime.Now;
                    title.Title.TotalSeconds = (uint)timeSpan.TotalSeconds;
                    Owner.Send(stream.CreateTitleStorage(title));
                    title.dwparam1 = 100;
                    title.ActionID = MsgTitleStorage.Action.FullLoad;
                    Owner.Send(stream.CreateTitleStorage(title));
                }
            }
        }
        public void SendSpecialTitleTime(ServerSockets.Packet stream)
        {
            foreach (var _title in TitleWithTime.Values)
            {
                Database.TitleStorage dbtitle;
                if (Database.TitleStorage.Titles.TryGetValue((uint)_title.titleID, out dbtitle))
                {
                    MsgTitleStorage.TitleStorage title = new MsgTitleStorage.TitleStorage();
                    title.ActionID = MsgTitleStorage.Action.UpdateScore;
                    title.dwparam1 = GetTitlesScore();
                    title.dwparam2 = dbtitle.ID;
                    title.dwparam3 = dbtitle.SubID;
                    title.Title = new MsgTitleStorage.Title();
                    title.Title.ID = dbtitle.ID;
                    title.Title.SubId = dbtitle.SubID;
                    TimeSpan timeSpan = _title.DateStamp - DateTime.Now;
                    title.Title.TotalSeconds = (uint)timeSpan.TotalSeconds;
                    Owner.Send(stream.CreateTitleStorage(title));
                    title.dwparam1 = 100;
                    title.ActionID = MsgTitleStorage.Action.FullLoad;
                    Owner.Send(stream.CreateTitleStorage(title));
                }
            }

        }
        public WardRobeTitle[] TitlesWithTime
        {
            get
            {
                return TitleWithTime.Values.Where(title => title.TotalSeconds > 0).ToArray();
            }
        }
        public void AddSpecialTitle(MsgTitleStorage.TitleType type, ServerSockets.Packet stream)
        {
            Database.TitleStorage dbtitle;
            if (Database.TitleStorage.Titles.TryGetValue((uint)type, out dbtitle))
            {

                if (!SpecialTitles.Contains(type))
                {
                    MsgTitleStorage.TitleStorage title = new MsgTitleStorage.TitleStorage();
                    title.ActionID = MsgTitleStorage.Action.UpdateScore;
                    title.dwparam1 = GetTitlesScore();
                    title.dwparam2 = dbtitle.ID;
                    title.dwparam3 = dbtitle.SubID;
                    title.Title = new MsgTitleStorage.Title();
                    title.Title.ID = dbtitle.ID;
                    title.Title.SubId = dbtitle.SubID;

                    Owner.Send(stream.CreateTitleStorage(title));
                    title.dwparam1 = 100;
                    title.ActionID = MsgTitleStorage.Action.FullLoad;
                    Owner.Send(stream.CreateTitleStorage(title));
                    SpecialTitles.Add(type);
                }
            }
        }
        public void AddSpecialFootprint(MsgTitleStorage.Footprint type, ServerSockets.Packet stream)
        {
            Database.TitleStorage dbtitle;
            if (Database.TitleStorage.Titles.TryGetValue((uint)type, out dbtitle))
            {
                if (!Footprint.Contains(type))
                {
                    MsgTitleStorage.TitleStorage title = new MsgTitleStorage.TitleStorage();
                    title.ActionID = MsgTitleStorage.Action.UpdateScore;
                    title.dwparam1 = GetFootprintScore();
                    title.dwparam2 = dbtitle.ID;
                    title.dwparam3 = dbtitle.SubID;
                    title.Title = new MsgTitleStorage.Title();
                    title.Title.ID = dbtitle.ID;
                    title.Title.SubId = dbtitle.SubID;

                    Owner.Send(stream.CreateTitleStorage(title));
                    title.dwparam1 = 100;
                    title.ActionID = MsgTitleStorage.Action.FullLoad;
                    Owner.Send(stream.CreateTitleStorage(title));
                    Footprint.Add(type);
                }
            }
        }
        public void AddSpecialKillEffectsAction(MsgTitleStorage.KillEffects type, ServerSockets.Packet stream)
        {
            Database.TitleStorage dbtitle;
            if (Database.TitleStorage.Titles.TryGetValue((uint)type, out dbtitle))
            {
                lock (KillEffectsAction)
                {
                    if (!KillEffectsAction.Contains(type))
                    {
                        MsgTitleStorage.TitleStorage title = new MsgTitleStorage.TitleStorage();
                        title.ActionID = MsgTitleStorage.Action.UpdateScore;
                        title.dwparam1 = GetHaloActionScore();
                        title.dwparam2 = dbtitle.ID;
                        title.dwparam3 = dbtitle.SubID;
                        title.Title = new MsgTitleStorage.Title();
                        title.Title.ID = dbtitle.ID;
                        title.Title.SubId = dbtitle.SubID;
                        Owner.Send(stream.CreateTitleStorage(title));
                        title.dwparam1 = 100;
                        title.ActionID = MsgTitleStorage.Action.FullLoad;
                        Owner.Send(stream.CreateTitleStorage(title));
                        KillEffectsAction.Add(type);
                    }
                }
            }
        }
        public void AddSpecialHaloAction(MsgTitleStorage.HaloAction type, ServerSockets.Packet stream)
        {
            Database.TitleStorage dbtitle;
            if (Database.TitleStorage.Titles.TryGetValue((uint)type, out dbtitle))
            {
                lock (HaloAction)
                {
                    if (!HaloAction.Contains(type))
                    {
                        MsgTitleStorage.TitleStorage title = new MsgTitleStorage.TitleStorage();
                        title.ActionID = MsgTitleStorage.Action.UpdateScore;
                        title.dwparam1 = GetHaloActionScore();
                        title.dwparam2 = dbtitle.ID;
                        title.dwparam3 = dbtitle.SubID;
                        title.Title = new MsgTitleStorage.Title();
                        title.Title.ID = dbtitle.ID;
                        title.Title.SubId = dbtitle.SubID;

                        Owner.Send(stream.CreateTitleStorage(title));
                        title.dwparam1 = 100;
                        title.ActionID = MsgTitleStorage.Action.FullLoad;
                        Owner.Send(stream.CreateTitleStorage(title));
                        HaloAction.Add(type);
                    }
                }
            }
        }

        public void AddAnySpecialTitle(uint id, ServerSockets.Packet stream)
        {
            if (id >= 4000 && id < 7000)
                AddSpecialWings((MsgTitleStorage.WingsType)id, stream);
            else if (id >= 8000 && id < 9000)
                AddSpecialHalo((MsgTitleStorage.HaloType)id, stream);
            else if (id >= 9000 && id < 9500)
                AddSpecialFootprint((MsgTitleStorage.Footprint)id, stream);
            else if (id >= 9500 && id < 10000)
                AddSpecialHaloAction((MsgTitleStorage.HaloAction)id, stream);
            else if (id >= 10000)
                AddSpecialKillEffectsAction((MsgTitleStorage.KillEffects)id, stream);
            else
                AddSpecialTitle((MsgTitleStorage.TitleType)id, stream);
        }

        public void AddSpecialHalo(MsgTitleStorage.HaloType type, ServerSockets.Packet stream)
        {
            Database.TitleStorage dbtitle;
            if (Database.TitleStorage.Titles.TryGetValue((uint)type, out dbtitle))
            {
                if (!HaloTitles.Contains(type))
                {
                    MsgTitleStorage.TitleStorage title = new MsgTitleStorage.TitleStorage();
                    title.ActionID = MsgTitleStorage.Action.UpdateScore;
                    title.dwparam1 = GetHaloScore();
                    title.dwparam2 = dbtitle.ID;
                    title.dwparam3 = dbtitle.SubID;
                    title.Title = new MsgTitleStorage.Title();
                    title.Title.ID = dbtitle.ID;
                    title.Title.SubId = dbtitle.SubID;

                    Owner.Send(stream.CreateTitleStorage(title));
                    title.dwparam1 = 100;
                    title.ActionID = MsgTitleStorage.Action.FullLoad;
                    Owner.Send(stream.CreateTitleStorage(title));
                    HaloTitles.Add(type);
                }
            }
        }
        public void AddSpecialWings(MsgTitleStorage.WingsType type, ServerSockets.Packet stream)
        {
            Database.TitleStorage dbtitle;
            if (Database.TitleStorage.Titles.TryGetValue((uint)type, out dbtitle))
            {
                if (!WingsTitles.Contains(type))
                {
                    MsgTitleStorage.TitleStorage title = new MsgTitleStorage.TitleStorage();
                    title.ActionID = MsgTitleStorage.Action.UpdateScore;
                    title.dwparam1 = GetWingsScore();
                    title.dwparam2 = dbtitle.ID;
                    title.dwparam3 = dbtitle.SubID;
                    title.Title = new MsgTitleStorage.Title();
                    title.Title.ID = dbtitle.ID;
                    title.Title.SubId = dbtitle.SubID;

                    Owner.Send(stream.CreateTitleStorage(title));
                    title.dwparam1 = 100;
                    title.ActionID = MsgTitleStorage.Action.FullLoad;
                    Owner.Send(stream.CreateTitleStorage(title));
                    WingsTitles.Add(type);
                }
            }
        }


        public void SendSpecialTitle(ServerSockets.Packet stream)
        {
            foreach (var _title in SpecialTitles)
            {
                Database.TitleStorage dbtitle;
                if (Database.TitleStorage.Titles.TryGetValue((uint)_title, out dbtitle))
                {
                    MsgTitleStorage.TitleStorage title = new MsgTitleStorage.TitleStorage();
                    title.ActionID = MsgTitleStorage.Action.UpdateScore;
                    title.dwparam1 = GetTitlesScore();
                    title.dwparam2 = dbtitle.ID;
                    title.dwparam3 = dbtitle.SubID;
                    title.Title = new MsgTitleStorage.Title();
                    title.Title.ID = dbtitle.ID;
                    title.Title.SubId = dbtitle.SubID;

                    Owner.Send(stream.CreateTitleStorage(title));
                    title.dwparam1 = 100;
                    title.ActionID = MsgTitleStorage.Action.FullLoad;
                    Owner.Send(stream.CreateTitleStorage(title));
                    // SpecialTitles.Add(_title);
                }
            }
            foreach (var _title in HaloTitles)
            {
                Database.TitleStorage dbtitle;
                if (Database.TitleStorage.Titles.TryGetValue((uint)_title, out dbtitle))
                {
                    MsgTitleStorage.TitleStorage title = new MsgTitleStorage.TitleStorage();
                    title.ActionID = MsgTitleStorage.Action.UpdateScore;
                    title.dwparam1 = GetHaloScore();
                    title.dwparam2 = dbtitle.ID;
                    title.dwparam3 = dbtitle.SubID;
                    title.Title = new MsgTitleStorage.Title();
                    title.Title.ID = dbtitle.ID;
                    title.Title.SubId = dbtitle.SubID;

                    Owner.Send(stream.CreateTitleStorage(title));
                    title.dwparam1 = 100;
                    title.ActionID = MsgTitleStorage.Action.FullLoad;
                    Owner.Send(stream.CreateTitleStorage(title));
                    // SpecialTitles.Add(_title);
                }
            }
            foreach (var _title in WingsTitles)
            {
                Database.TitleStorage dbtitle;
                if (Database.TitleStorage.Titles.TryGetValue((uint)_title, out dbtitle))
                {
                    MsgTitleStorage.TitleStorage title = new MsgTitleStorage.TitleStorage();
                    title.ActionID = MsgTitleStorage.Action.UpdateScore;
                    title.dwparam1 = GetWingsScore();
                    title.dwparam2 = dbtitle.ID;
                    title.dwparam3 = dbtitle.SubID;
                    title.Title = new MsgTitleStorage.Title();
                    title.Title.ID = dbtitle.ID;
                    title.Title.SubId = dbtitle.SubID;

                    Owner.Send(stream.CreateTitleStorage(title));
                    title.dwparam1 = 100;
                    title.ActionID = MsgTitleStorage.Action.FullLoad;
                    Owner.Send(stream.CreateTitleStorage(title));
                    // SpecialTitles.Add(_title);
                }
            }
            foreach (var _title in HaloAction)
            {
                Database.TitleStorage dbtitle;
                if (Database.TitleStorage.Titles.TryGetValue((uint)_title, out dbtitle))
                {
                    MsgTitleStorage.TitleStorage title = new MsgTitleStorage.TitleStorage();
                    title.ActionID = MsgTitleStorage.Action.UpdateScore;
                    title.dwparam1 = GetHaloActionScore();
                    title.dwparam2 = dbtitle.ID;
                    title.dwparam3 = dbtitle.SubID;
                    title.Title = new MsgTitleStorage.Title();
                    title.Title.ID = dbtitle.ID;
                    title.Title.SubId = dbtitle.SubID;

                    Owner.Send(stream.CreateTitleStorage(title));
                    title.dwparam1 = 100;
                    title.ActionID = MsgTitleStorage.Action.FullLoad;
                    Owner.Send(stream.CreateTitleStorage(title));
                }
            }
            foreach (var _title in Footprint)
            {
                Database.TitleStorage dbtitle;
                if (Database.TitleStorage.Titles.TryGetValue((uint)_title, out dbtitle))
                {
                    MsgTitleStorage.TitleStorage title = new MsgTitleStorage.TitleStorage();
                    title.ActionID = MsgTitleStorage.Action.UpdateScore;
                    title.dwparam1 = GetFootprintScore();
                    title.dwparam2 = dbtitle.ID;
                    title.dwparam3 = dbtitle.SubID;
                    title.Title = new MsgTitleStorage.Title();
                    title.Title.ID = dbtitle.ID;
                    title.Title.SubId = dbtitle.SubID;

                    Owner.Send(stream.CreateTitleStorage(title));
                    title.dwparam1 = 100;
                    title.ActionID = MsgTitleStorage.Action.FullLoad;
                    Owner.Send(stream.CreateTitleStorage(title));
                    // SpecialTitles.Add(_title);
                }
            }
        }

        public void ActiveSpecialTitles(ServerSockets.Packet stream)
        {
            Database.TitleStorage dbtitle;
            if (Database.TitleStorage.Titles.TryGetValue((uint)(SpecialTitleID / 10000), out dbtitle))
            {
                WardRobeTitle value;
                if (TitleWithTime.TryGetValue((uint)(SpecialTitleID / 10000), out  value))
                {
                    MsgTitleStorage.TitleStorage title = new MsgTitleStorage.TitleStorage();
                    title.ActionID = MsgTitleStorage.Action.UpdateScore;
                    title.dwparam1 = GetHaloScore();
                    title.dwparam2 = dbtitle.ID;
                    title.dwparam3 = dbtitle.SubID;
                    title.Title = new MsgTitleStorage.Title();
                    title.Title.ID = dbtitle.ID;
                    title.Title.SubId = dbtitle.SubID;
                    title.Title.dwparam1 = 100;
                    TimeSpan timeSpan = value.DateStamp - DateTime.Now;
                    title.Title.TotalSeconds = (uint)timeSpan.TotalSeconds;
                    Owner.Send(stream.CreateTitleStorage(title));

                }
                else
                {
                    MsgTitleStorage.TitleStorage title = new MsgTitleStorage.TitleStorage();
                    title.ActionID = MsgTitleStorage.Action.UpdateScore;
                    title.dwparam1 = GetTitlesScore();
                    title.dwparam2 = dbtitle.ID;
                    title.dwparam3 = dbtitle.SubID;
                    title.Title = new MsgTitleStorage.Title();
                    title.Title.ID = dbtitle.ID;
                    title.Title.SubId = dbtitle.SubID;
                    title.Title.dwparam1 = 100;
                    Owner.Send(stream.CreateTitleStorage(title));
                }
            }
            if (Database.TitleStorage.Titles.TryGetValue((uint)(SpecialWingID / 10000), out dbtitle))
            {
                WardRobeTitle value;
                if (TitleWithTime.TryGetValue((uint)(SpecialWingID / 10000), out  value))
                {
                    MsgTitleStorage.TitleStorage title = new MsgTitleStorage.TitleStorage();
                    title.ActionID = MsgTitleStorage.Action.UpdateScore;
                    title.dwparam1 = GetHaloScore();
                    title.dwparam2 = dbtitle.ID;
                    title.dwparam3 = dbtitle.SubID;
                    title.Title = new MsgTitleStorage.Title();
                    title.Title.ID = dbtitle.ID;
                    title.Title.SubId = dbtitle.SubID;
                    title.Title.dwparam1 = 100;
                    TimeSpan timeSpan = value.DateStamp - DateTime.Now;
                    title.Title.TotalSeconds = (uint)timeSpan.TotalSeconds;
                    Owner.Send(stream.CreateTitleStorage(title));

                }
                else
                {
                    MsgTitleStorage.TitleStorage title = new MsgTitleStorage.TitleStorage();
                    title.ActionID = MsgTitleStorage.Action.UpdateScore;
                    title.dwparam1 = GetWingsScore();
                    title.dwparam2 = dbtitle.ID;
                    title.dwparam3 = dbtitle.SubID;
                    title.Title = new MsgTitleStorage.Title();
                    title.Title.ID = dbtitle.ID;
                    title.Title.SubId = dbtitle.SubID;
                    title.Title.dwparam1 = 1;
                    Owner.Send(stream.CreateTitleStorage(title));
                }

            }
            if (Database.TitleStorage.Titles.TryGetValue((uint)(SpecialHaloID / 10000), out dbtitle))
            {
                WardRobeTitle value;
                if (TitleWithTime.TryGetValue((uint)(SpecialHaloID / 10000), out  value))
                {
                    MsgTitleStorage.TitleStorage title = new MsgTitleStorage.TitleStorage();
                    title.ActionID = MsgTitleStorage.Action.UpdateScore;
                    title.dwparam1 = GetHaloScore();
                    title.dwparam2 = dbtitle.ID;
                    title.dwparam3 = dbtitle.SubID;
                    title.Title = new MsgTitleStorage.Title();
                    title.Title.ID = dbtitle.ID;
                    title.Title.SubId = dbtitle.SubID;
                    title.Title.dwparam1 = 100;
                    TimeSpan timeSpan = value.DateStamp - DateTime.Now;
                    title.Title.TotalSeconds = (uint)timeSpan.TotalSeconds;
                    Owner.Send(stream.CreateTitleStorage(title));

                }
                else
                {
                    MsgTitleStorage.TitleStorage title = new MsgTitleStorage.TitleStorage();
                    title.ActionID = MsgTitleStorage.Action.UpdateScore;
                    title.dwparam1 = GetHaloScore();
                    title.dwparam2 = dbtitle.ID;
                    title.dwparam3 = dbtitle.SubID;
                    title.Title = new MsgTitleStorage.Title();
                    title.Title.ID = dbtitle.ID;
                    title.Title.SubId = dbtitle.SubID;
                    title.Title.dwparam1 = 100;
                    Owner.Send(stream.CreateTitleStorage(title));
                }

            }
            if (Database.TitleStorage.Titles.TryGetValue((uint)(SpecialFootprintID / 10000), out dbtitle))
            {

                MsgTitleStorage.TitleStorage pQuery = new MsgTitleStorage.TitleStorage();
                Owner.Send(stream.CreateTitleStorage(pQuery));
                pQuery.ActionID = MsgTitleStorage.Action.UseTitle;
                pQuery.Title = new MsgTitleStorage.Title();
                pQuery.Title.ID = dbtitle.ID;
                pQuery.Title.SubId = dbtitle.SubID;
                pQuery.Title.dwparam1 = 1;
                Owner.Send(stream.CreateTitleStorage(pQuery));
                Owner.Player.View.SendView(Owner.Player.GetArray(stream, false), false);

            }
            if (Database.TitleStorage.Titles.TryGetValue((uint)(SpecialHaloAction / 10000), out dbtitle))
            {
                MsgTitleStorage.TitleStorage pQuery = new MsgTitleStorage.TitleStorage();
                Owner.Send(stream.CreateTitleStorage(pQuery));
                pQuery.ActionID = MsgTitleStorage.Action.UseTitle;
                pQuery.Title = new MsgTitleStorage.Title();
                pQuery.Title.ID = dbtitle.ID;
                pQuery.Title.SubId = dbtitle.SubID;
                pQuery.Title.dwparam1 = 1;
                Owner.Send(stream.CreateTitleStorage(pQuery));
                Owner.Player.View.SendView(Owner.Player.GetArray(stream, false), false);

            }
        }
        public uint TableBetDice = 0;
        public uint SpecialFootprintID = 0;
        public uint SpecialHaloAction = 0;
        public uint SpecialFootprintScore = 0;
        public uint SpecialHaloActionScore = 0;

        public uint SpecialTitleID = 0;
        public uint SpecialHaloID = 0;
        public uint SpecialWingID = 0;
        public uint SpecialTitleScore = 0;
        public uint PistilAromaID = 0;
        public void RemoveSpecialTitle(MsgTitleStorage.TitleType type, ServerSockets.Packet stream)
        {
            try
            {
                Database.TitleStorage dbtitle;
                if (Database.TitleStorage.Titles.TryGetValue((uint)type, out dbtitle))
                {
                    if (!SpecialTitles.Contains(type) && SpecialTitleID / 10000 != (uint)type && SpecialWingID / 10000 != (uint)type) return;// QahrMobile: no "You lost the title" for a title the player never had (also runs before the mobile flag is set at login)
                    var idx = SpecialTitles.IndexOf(type);
                    if (idx >= 0 && idx < SpecialTitles.Count && SpecialTitles.Contains(type))
                        SpecialTitles.Remove(type);
                    MsgTitleStorage.TitleStorage title = new MsgTitleStorage.TitleStorage();
                    title.ActionID = MsgTitleStorage.Action.UnEquip;
                    title.dwparam2 = dbtitle.ID;
                    title.dwparam3 = dbtitle.SubID;
                    Owner.Send(stream.CreateTitleStorage(title));
                    title.dwparam1 = GetTitlesScore();
                    title.Title = new MsgTitleStorage.Title();
                    title.Title.ID = dbtitle.ID;
                    title.Title.SubId = dbtitle.SubID;
                    title.Title.dwparam1 = 0;
                    Owner.Send(stream.CreateTitleStorage(title));
                    title.ActionID = MsgTitleStorage.Action.UpdateScore;
                    Owner.Send(stream.CreateTitleStorage(title));
                    title.ActionID = MsgTitleStorage.Action.RemoveTitle;
                    Owner.Send(stream.CreateTitleStorage(title));

                    if (SpecialTitleID / 10000 == (uint)type)
                        SpecialTitleID = SpecialTitleScore = 0;
                    else if (SpecialWingID / 10000 == (uint)type)
                        SpecialWingID = 0;

                }
            }
            catch
            {

            }
        }
        public void RemoveSpecialWings(MsgTitleStorage.WingsType type, ServerSockets.Packet stream)
        {
            Database.TitleStorage dbtitle;
            if (Database.TitleStorage.Titles.TryGetValue((uint)type, out dbtitle))
            {
                if (!WingsTitles.Contains(type) && SpecialWingID / 10000 != (uint)type) return;// QahrMobile: nothing to remove (no "You lost the ... Wings" for players who never had them)
                if (WingsTitles.Contains(type))
                    WingsTitles.Remove(type);
                MsgTitleStorage.TitleStorage title = new MsgTitleStorage.TitleStorage();
                title.ActionID = MsgTitleStorage.Action.UnEquip;
                title.dwparam2 = dbtitle.ID;
                title.dwparam3 = dbtitle.SubID;
                Owner.Send(stream.CreateTitleStorage(title));
                title.dwparam1 = GetWingsScore();
                title.Title = new MsgTitleStorage.Title();
                title.Title.ID = dbtitle.ID;
                title.Title.SubId = dbtitle.SubID;
                title.Title.dwparam1 = 0;
                Owner.Send(stream.CreateTitleStorage(title));
                title.ActionID = MsgTitleStorage.Action.UpdateScore;
                Owner.Send(stream.CreateTitleStorage(title));
                title.ActionID = MsgTitleStorage.Action.RemoveTitle;
                Owner.Send(stream.CreateTitleStorage(title));
                if (SpecialWingID / 10000 == (uint)type)
                    SpecialWingID = 0;

            }

        }
        public void RemoveFootprint(MsgTitleStorage.Footprint type, ServerSockets.Packet stream)
        {
            Database.TitleStorage dbtitle;
            if (Database.TitleStorage.Titles.TryGetValue((uint)type, out dbtitle))
            {
                if (Footprint.Contains(type))
                    Footprint.Remove(type);
                MsgTitleStorage.TitleStorage title = new MsgTitleStorage.TitleStorage();
                title.ActionID = MsgTitleStorage.Action.UnEquip;
                title.dwparam2 = dbtitle.ID;
                title.dwparam3 = dbtitle.SubID;
                Owner.Send(stream.CreateTitleStorage(title));
                title.dwparam1 = GetHaloActionScore();
                title.Title = new MsgTitleStorage.Title();
                title.Title.ID = dbtitle.ID;
                title.Title.SubId = dbtitle.SubID;
                title.Title.dwparam1 = 0;
                Owner.Send(stream.CreateTitleStorage(title));
                title.ActionID = MsgTitleStorage.Action.UpdateScore;
                Owner.Send(stream.CreateTitleStorage(title));
                title.ActionID = MsgTitleStorage.Action.RemoveTitle;
                Owner.Send(stream.CreateTitleStorage(title));
                if (SpecialFootprintID / 10000 == (uint)type)
                {
                    SpecialFootprintID = 0;
                    SpecialFootprintScore = 0;
                }
            }

        }
        public void RemoveHaloAction(MsgTitleStorage.HaloAction type, ServerSockets.Packet stream)
        {
            Database.TitleStorage dbtitle;
            if (Database.TitleStorage.Titles.TryGetValue((uint)type, out dbtitle))
            {
                if (HaloAction.Contains(type))
                    HaloAction.Remove(type);
                MsgTitleStorage.TitleStorage title = new MsgTitleStorage.TitleStorage();
                title.ActionID = MsgTitleStorage.Action.UnEquip;
                title.dwparam2 = dbtitle.ID;
                title.dwparam3 = dbtitle.SubID;
                Owner.Send(stream.CreateTitleStorage(title));
                title.dwparam1 = GetHaloActionScore();
                title.Title = new MsgTitleStorage.Title();
                title.Title.ID = dbtitle.ID;
                title.Title.SubId = dbtitle.SubID;
                title.Title.dwparam1 = 0;
                Owner.Send(stream.CreateTitleStorage(title));
                title.ActionID = MsgTitleStorage.Action.UpdateScore;
                Owner.Send(stream.CreateTitleStorage(title));
                title.ActionID = MsgTitleStorage.Action.RemoveTitle;
                Owner.Send(stream.CreateTitleStorage(title));
                if (SpecialHaloAction / 10000 == (uint)type)
                {
                    SpecialHaloAction = 0;
                    SpecialHaloActionScore = 0;
                }
            }

        }
        public void RemoveSpecialTitleTime(uint type, ServerSockets.Packet stream)
        {
            Database.TitleStorage dbtitle;
            if (Database.TitleStorage.Titles.TryGetValue((uint)type, out dbtitle))
            {
                if (TitleWithTime.ContainsKey((uint)type))
                    TitleWithTime.Remove(((uint)type));
                MsgTitleStorage.TitleStorage title = new MsgTitleStorage.TitleStorage();
                title.ActionID = MsgTitleStorage.Action.UnEquip;
                title.dwparam2 = dbtitle.ID;
                title.dwparam3 = dbtitle.SubID;
                Owner.Send(stream.CreateTitleStorage(title));
                title.dwparam1 = GetTitlesScore();
                title.Title = new MsgTitleStorage.Title();
                title.Title.ID = dbtitle.ID;
                title.Title.SubId = dbtitle.SubID;
                title.Title.dwparam1 = 0;
                Owner.Send(stream.CreateTitleStorage(title));
                title.ActionID = MsgTitleStorage.Action.UpdateScore;
                Owner.Send(stream.CreateTitleStorage(title));
                title.ActionID = MsgTitleStorage.Action.RemoveTitle;
                Owner.Send(stream.CreateTitleStorage(title));

                if (SpecialTitleID / 10000 == (uint)type)
                    SpecialTitleID = SpecialTitleScore = 0;
                else if (SpecialWingID / 10000 == (uint)type)
                    SpecialWingID = 0;

            }

        }

        public bool StartVote = false;

        public DateTime StartVoteStamp = new DateTime();

        public bool OnAttackPotion = false;
        public DateTime OnAttackPotionStamp = new DateTime();
        public void ActiveAttackPotion(int Timer)
        {
            OnAttackPotion = true;
            OnAttackPotionStamp = DateTime.Now.AddMinutes(Timer);
            AddFlag(MsgUpdate.Flags.Stigma, 60, true);
#if Arabic
              Owner.SendSysMesage("Your attack will increase during the next 30 minutes.", MsgMessage.ChatMode.System);
#else
            Owner.SendSysMesage("Your attack will increase during the next 30 minutes.", MsgMessage.ChatMode.System);
#endif

        }

        public bool OnDefensePotion = false;
        public DateTime OnDefensePotionStamp = new DateTime();
        public void ActiveDefensePotion(int Timer)
        {
            OnDefensePotion = true;
            OnDefensePotionStamp = DateTime.Now.AddMinutes(Timer);
            AddFlag(MsgUpdate.Flags.Shield, 60, true);
#if Arabic
               Owner.SendSysMesage("Your defense will increase during the next 30 minutes.", MsgMessage.ChatMode.System);
#else
            Owner.SendSysMesage("Your defense will increase during the next 30 minutes.", MsgMessage.ChatMode.System);
#endif

        }


        public bool WaveofBlood = false;
        public DateTime WaveofBloodStamp = new DateTime();
        public DateTime AutoBackfire = new DateTime();

        public bool AllowDynamic { get; set; }
        public uint DailyMagnoliaChance = 0;
        public uint DailyMagnoliaItemId = 0;

        public uint DailySpiritBeadCount = 0;

        public uint DailySpiritBeadItem = 0;
        public uint DailyRareChance = 0;
        public uint DailyHeavenChance = 0;

        public uint TodayChampionPoints = 0;
        public uint HistoryChampionPoints = 0;

        uint _ChampionPoints;
        public uint ChampionPoints
        {

            get
            {
                return _ChampionPoints;
            }
            set
            {

                _ChampionPoints = value;
                if (_ChampionPoints > HistoryChampionPoints)
                    HistoryChampionPoints = value;
            }
        }

        public void AddChampionPoints(uint value, bool settodayvalue = true)
        {
            if (settodayvalue)
            {
                if (TodayChampionPoints > 650)
                {
                    Owner.SendSysMesage("Your Champion Points have reached the maximum amount of 650 points. You can collect more points tomorrow.");
                    return;
                }
            }
            ChampionPoints += value;
            TodayChampionPoints += value;
        }

        public DateTime PickStamp = DateTime.Now;
        public bool ActivePick = false;
        public byte Blessing = 0;
        public byte StarEpicItemPirate = 0;
        public byte StarEpicItemPirateCount = 0;
        public uint DragonFurnace, DragonFurnaceForge, DragonFurnaceForgeCount;
        public uint PokerA, PokerB;
        public void AddBlessing(ServerSockets.Packet stream, string Name, ushort timer, byte Bless)
        {
            PickStamp = DateTime.Now.AddSeconds(timer);
            Owner.Send(stream.ActionPick(UID, 1, timer, "Stay for luck"));
            ActivePick = true;
            Blessing = Bless;
            ActionQuery action = new ActionQuery()
            {
                ObjId = UID,
                Type = 1165,
                PositionX = 277,
                PositionY = 2050
            };
            Owner.Send(stream.ActionCreate(action));
        }
        public void EpicPirateQuest(ServerSockets.Packet stream, string Name, ushort timer, byte Star, byte Count)
        {
            PickStamp = DateTime.Now.AddSeconds(timer);
            Owner.Send(stream.ActionPick(UID, 1, timer, Name));
            ActivePick = true;
            StarEpicItemPirate = Star;
            StarEpicItemPirateCount = Count;
            ActionQuery action = new ActionQuery()
            {
                ObjId = UID,
                Type = 1165,
                PositionX = 277,
                PositionY = 2050
            };
            Owner.Send(stream.ActionCreate(action));
        }
        public void AddPick(ServerSockets.Packet stream, string Name, ushort timer)
        {
            PickStamp = DateTime.Now.AddSeconds(timer);
            Owner.Send(stream.ActionPick(UID, 1, timer, Name));
            ActivePick = true;

            ActionQuery action = new ActionQuery()
            {
                ObjId = UID,
                Type = 1165,
                PositionX = 277,
                PositionY = 2050
            };
            Owner.Send(stream.ActionCreate(action));
        }
        public void RemovePick(ServerSockets.Packet stream)
        {
            ActivePick = false;
            Owner.Send(stream.ActionPick(UID, 2, 0, "Interrupted"));
        }

        public uint EpicQuestChance = 0;
        public uint EpicQuestTwo_ndChance = 0;
        public uint EpicQuestThree_rdChance = 0;
        public uint EpicNinjaQuestChance = 0;
        public uint EpicNinjaTwoQuestChance = 0;
        public uint EpicNinjaThreeQuestChance = 0;
        public uint EpicPirateQuestChance = 0;
        public uint IndexInScreen { get; set; }
        public bool IsTrap() { return false; }

        public byte Away = 0;
        public uint FlagsPointss = 0;
        public uint WorldPoints = 0;
        public byte ChiToken = 0;
        public byte Vote = 0;
        public uint CastlePoint = 0;
        public byte JiangToken = 0;
        public uint OnlinePointsPK = 0;
        public bool ReviveProtection = false;
        public uint OnlineMinutes = 0;
        public uint GodzillaPoints = 0;
        public uint Emoji = 0;
        public uint ChatSkinID = 0;
        public DateTime OnlineStamp = DateTime.Now;
        public uint OnlineHours
        {
            get { return OnlineMinutes / 60; }
        }
        public Role.Instance.InnerPower InnerPower;

        public Game.MsgTournaments.MsgSteedRace.UsableRacePotion[] RacePotions = null;

        public uint KillerPkPoints = 0;
        public uint XtremePkPoints = 0;

        public uint DragonWarHits = 0;
        public uint DragonWarScore = 0;
        public uint ScoreHuntCoins = 0;

        public int StarFlow { get; set; }

        public uint TeamDeathMacthKills = 0;
        public uint TournamentKills = 0;

        public uint SpecialGarment = 0;
        public void RemoveSpecialGarment(ServerSockets.Packet stream)
        {
            SpecialGarment = 0;
            GarmentId = 0;
            MsgGameItem item;
            if (Owner.Equipment.TryGetEquip(Flags.ConquerItem.Garment, out item))
            {
                Owner.Send(stream.ItemUsageCreate(MsgItemUsuagePacket.ItemUsuageID.Equip, item.UID, (ushort)Flags.ConquerItem.Garment, 0, 0, 0, 0, 0));
                item.Mode = Flags.ItemMode.AddItem;
                item.Send(Owner, stream);
            }
            Owner.Equipment.QueryEquipment(Owner.Equipment.Alternante);
        }
        public void AddSpecialGarment(ServerSockets.Packet stream, uint ID)
        {


            SpecialGarment = ID;
            GarmentId = SpecialGarment;
            Owner.Send(stream.ItemUsageCreate(MsgItemUsuagePacket.ItemUsuageID.Equip, uint.MaxValue - 1, (ushort)Flags.ConquerItem.Garment, 0, 0, 0, 0, 0));
            Game.MsgServer.MsgGameItem item = new MsgGameItem();
            item.ITEM_ID = ID;
            item.Mode = Flags.ItemMode.AddItem;
            item.UID = uint.MaxValue - 1;
            item.Color = Flags.Color.Red;
            item.Position = (ushort)Flags.ConquerItem.Garment;
            item.Durability = Pool.ItemsBase[ID].Durability;
            item.Send(Owner, stream);
            Owner.Equipment.AppendItems(true, Owner.Equipment.CurentEquip, stream);
            View.SendView(GetArray(stream, false), false);
        }
        internal string AgatesString()
        {
            string str1 = "";
            foreach (MsgGameItem MsgGameItem in this.Owner.Inventory.ClientItems.Values.Where<MsgGameItem>((Func<MsgGameItem, bool>)(e => e.ITEM_ID == 720828U)))
            {
                str1 = str1 + MsgGameItem.UID.ToString() + "#";
                foreach (string str2 in MsgGameItem.Agate_map.Values)
                    str1 = str1 + str2 + "#";
                str1 += "$";
            }
            foreach (ConcurrentDictionary<uint, MsgGameItem> concurrentDictionary in (IEnumerable<ConcurrentDictionary<uint, MsgGameItem>>)this.Owner.Warehouse.ClientItems.Values)
            {
                foreach (MsgGameItem MsgGameItem in concurrentDictionary.Values.Where<MsgGameItem>((Func<MsgGameItem, bool>)(e => e.ITEM_ID == 720828U)))
                {
                    str1 = str1 + MsgGameItem.UID.ToString() + "#";
                    foreach (string str3 in MsgGameItem.Agate_map.Values)
                        str1 = str1 + str3 + "#";
                    str1 += "$";
                }
            }
            return str1;
        }

        internal void LoadAgates(string str)
        {
            if (!(str != ""))
                return;
            string str1 = str;
            string[] separator = new string[1] { "$" };
            foreach (string str2 in str1.Split(separator, StringSplitOptions.RemoveEmptyEntries))
            {
                uint key = uint.Parse(str2.Split('#')[0]);
                string[] strArray = str2.Replace(key.ToString() + "#", "").Split(new string[1]
        {
          "#"
        }, StringSplitOptions.RemoveEmptyEntries);
                MsgGameItem MsgGameItem;
                if (this.Owner.Inventory.ClientItems.TryGetValue(key, out MsgGameItem))
                {
                    int num = 0;
                    foreach (string str3 in strArray)
                        MsgGameItem.Agate_map.Add((uint)num++, str3);
                }
                foreach (ConcurrentDictionary<uint, MsgGameItem> concurrentDictionary in (IEnumerable<ConcurrentDictionary<uint, MsgGameItem>>)this.Owner.Warehouse.ClientItems.Values)
                {
                    if (concurrentDictionary.TryGetValue(key, out MsgGameItem))
                    {
                        int num = 0;
                        foreach (string str4 in strArray)
                            MsgGameItem.Agate_map.Add((uint)num++, str4);
                    }
                }
            }
        }

        public DateTime StampArenaScore = new DateTime();

        public uint HitShoot = 0;
        public uint MisShoot = 0;
        public uint ArenaDeads = 0;
        public uint ArenaKills = 0;
        public uint ScoreBestFighter = 0;

        public uint KillersDisCity = 0;


        public byte TaoistPower = 0;
        public DateTime TaoistStampPower = DateTime.Now;

        public void UpdateTaoPower(ServerSockets.Packet stream)
        {
            if (Database.AtributesStatus.IsWater(Class) && TaoistPower == 10)
            {
                if (!ContainFlag(MsgUpdate.Flags.FullPowerWater))
                    AddFlag(MsgUpdate.Flags.FullPowerWater, StatusFlagsBigVector32.PermanentFlag, false);
            }
            else if (Database.AtributesStatus.IsFire(Class) && TaoistPower == 10)
            {
                if (!ContainFlag(MsgUpdate.Flags.FullPowerFire))
                    AddFlag(MsgUpdate.Flags.FullPowerFire, StatusFlagsBigVector32.PermanentFlag, false);
            }

            uint icon = Database.AtributesStatus.IsWater(Class) ? (uint)172 : (uint)173;

            Game.MsgServer.MsgUpdate packet = new Game.MsgServer.MsgUpdate(stream, UID, 1);
            stream = packet.Append(stream, MsgUpdate.DataType.AppendIcon, icon, 3, (uint)(TaoistPower * 30), 0);
            stream = packet.GetArray(stream);

            View.SendView(stream, true);
        }
        public void SendPowerTaoist(Client.GameClient user, ServerSockets.Packet stream)
        {
            if (TaoistPower > 0)
            {
                uint icon = Database.AtributesStatus.IsWater(Class) ? (uint)172 : (uint)173;

                Game.MsgServer.MsgUpdate packet = new Game.MsgServer.MsgUpdate(stream, UID, 1);
                stream = packet.Append(stream, MsgUpdate.DataType.AppendIcon, icon, 3, (uint)(TaoistPower * 30), 0);
                stream = packet.GetArray(stream);

                user.Send(stream);
            }
        }

        public void UpdateTaoistPower(DateTime now)
        {
            if (TaoistPower < 10 && Class > 10000)
            {
                if (now > TaoistStampPower.AddSeconds(9))
                {
                    Game.MsgServer.MsgGameItem GameItem;
                    if (Owner.Equipment.TryGetEquip(Flags.ConquerItem.RightWeapon, out GameItem))
                    {
                        if (Database.ItemType.IsTaoistEpicWeapon(GameItem.ITEM_ID))
                        {
                            TaoistPower += 1;
                            using (var rec = new ServerSockets.RecycledPacket())
                                UpdateTaoPower(rec.GetStream());
                        }
                    }
                    TaoistStampPower = DateTime.Now;
                }
            }
        }

        public uint AparenceType = 0;
        public unsafe void memcpy(void* dest, void* src, Int32 size)
        {
            Int32 count = size / sizeof(long);
            for (Int32 i = 0; i < count; i++)
                *(((long*)dest) + i) = *(((long*)src) + i);

            Int32 pos = size - (size % sizeof(long));
            for (Int32 i = 0; i < size % sizeof(long); i++)
                *(((Byte*)dest) + pos + i) = *(((Byte*)src) + pos + i);
        }
        public unsafe byte[] GetBytes(byte* packet)
        {
            int size = *(ushort*)(packet);
            size += 8;
            byte[] buff = new byte[size];
            fixed (byte* ptr = buff)
                memcpy(ptr, packet, size);
            return buff;
        }
        public uint TCCaptainTimes = 0;
        public byte BountyHallToday = 0;// [QuestFix 2026-10-13] Bounty Hall quests finished today (daily cap)
        public bool IsCheckedPass = false;
        public uint SecurityPassword = 0;
        public uint OnReset = 0;
        public DateTime ResetSecurityPassowrd = new DateTime();

        public DateTime LoginTimer = DateTime.Now;

        public bool InElitePk = false;
        public bool InTeamPk = false;
        public double DonatePoints;
        public uint SavePromote;
        public uint SavePromote1;
        public uint SavePromote2;
        public uint SavePromote3;
        public uint SavePromote4;
        public uint SavePromote5;
        public uint SavePromote6;
        public uint SavePromote7;
        public uint SavePromote8;
        public uint SavePromote9;
        public uint SavePromote10;
        public uint SavePromote11;
        public uint SavePromote12;
        public uint Online;
        public uint RamdanBag;
        public uint itemRamdanBag;
        public uint RewardPoints;
        public uint RacePoints = 0;
        public uint BattleFieldPoints = 0;


        public List<byte> Titles = new List<byte>();

        public unsafe void AddTitle(byte _title, bool aSwitch)
        {
            if (!Titles.Contains(_title))
            {
                Titles.Add(_title);

                using (var rec = new ServerSockets.RecycledPacket())
                {
                    var stream = rec.GetStream();

                    Owner.Send(stream.TitleCreate(UID, _title, MsgTitle.QueueTitle.Enqueue));

                }

                if (aSwitch)
                    SwitchTitle(_title);
            }
        }
        public unsafe void SwitchTitle(byte ntitle)
        {
            if (Titles.Contains(ntitle) || ntitle == 0)
            {
                MyTitle = ntitle;

                using (var rec = new ServerSockets.RecycledPacket())
                {
                    var stream = rec.GetStream();

                    Owner.Send(stream.TitleCreate(UID, ntitle, MsgTitle.QueueTitle.Change));

                }
            }
        }
        public unsafe byte MyTitle;

        public Flags.PKMode PreviousPkMode = Flags.PKMode.Capture;
        public unsafe void SetPkMode(Flags.PKMode pkmode)
        {
            PreviousPkMode = PkMode;
            PkMode = pkmode;

            using (var rec = new ServerSockets.RecycledPacket())
            {
                var stream = rec.GetStream();
                ActionQuery action = new ActionQuery()
                {
                    ObjId = UID,
                    dwParam = (uint)PkMode,
                    dwParam3 = (uint)PkMode,
                    Type = ActionType.SetPkMode
                };
                Owner.Send(stream.ActionCreate(action));
            }

        }
        public unsafe void RestorePkMode()
        {
            PkMode = PreviousPkMode;

            using (var rec = new ServerSockets.RecycledPacket())
            {
                var stream = rec.GetStream();
                ActionQuery action = new ActionQuery()
                {
                    ObjId = UID,
                    dwParam = (uint)PkMode,
                    dwParam3 = (uint)PkMode,
                    Type = ActionType.SetPkMode
                };
                Owner.Send(stream.ActionCreate(action));
            }
        }
        public DateTime EnlightenTime = new DateTime();

        public int CursedTimer = 0;
        public void AddCursed(int time)
        {
            if (time != 0)
            {
                if (ContainFlag(Game.MsgServer.MsgUpdate.Flags.Cursed))
                    RemoveFlag(Game.MsgServer.MsgUpdate.Flags.Cursed);

                CursedTimer += time;
                using (var rec = new ServerSockets.RecycledPacket())
                {
                    var stream = rec.GetStream();
                    SendUpdate(stream, CursedTimer, Game.MsgServer.MsgUpdate.DataType.CursedTimer);
                }
                AddFlag(Game.MsgServer.MsgUpdate.Flags.Cursed, CursedTimer, false, 1);
            }
        }

        public uint MyKillerUID;
        public string MyKillerName;

        public bool Delete = false;

        public uint VenomRate = 0;
        public Extensions.MyList<Clone> MyClones = new Extensions.MyList<Clone>();
        public DateTime CloneStamp;

        public ConcurrentDictionary<ushort, FloorSpell.ClientFloorSpells> FloorSpells = new ConcurrentDictionary<ushort, FloorSpell.ClientFloorSpells>();
        public void ClearFloor()
        {
            Queue<Role.FloorSpell> RemoveSpells = new Queue<Role.FloorSpell>();
            using (var rec = new ServerSockets.RecycledPacket())
            {
                var stream = rec.GetStream();
                if (Owner.Player.FloorSpells.Count != 0)
                {
                    foreach (var ID in Owner.Player.FloorSpells)
                    {
                        var spellclient = ID.Value;
                        var spells = spellclient.Spells.ToArray();
                        foreach (var spell in spells)
                        {
                            RemoveSpells.Enqueue(spell);

                            spell.FloorPacket.DropType = Game.MsgFloorItem.MsgDropID.RemoveEffect;
                            spell.FloorPacket.SendScreen(stream.ItemPacketCreate(spell.FloorPacket));
                        }
                        while (RemoveSpells.Count > 0)
                            spellclient.RemoveItem(RemoveSpells.Dequeue());
                        if (spellclient.Spells.Count == 0)
                        {
                            Role.FloorSpell.ClientFloorSpells FloorSpell;
                            Owner.Player.FloorSpells.TryRemove(spellclient.DBSkill.ID, out FloorSpell);
                        }
                    }
                }
            }
        }

        public ushort RandomSpell = 0;

        public bool DbTry = false;
        public byte AddJade = 0;
        public byte LotteryEntries;
        public byte LotteryHWND;
        public uint LotteryUseItem;
        public bool OldGate = false;
        public bool NewGate = false;
        public bool PlayOne = false;
        public bool PlayLunar = false;
        public byte GateID;
        public DateTime LotteryTime;
        public MsgGameItem[] LotteryItem = new MsgGameItem[10];
        public List<uint[]> LotteryList = new List<uint[]>();
        public bool Reincarnation = false;

        public byte JingTryngUltra = 0;
        public Instance.JiangHu MyJiangHu;
        public unsafe byte JiangHuTalent;
        public unsafe byte JiangHuActive;

        Instance.Clan.Member clanmemb;
        public Instance.Clan.Member MyClanMember
        {
            get
            {
                if (clanmemb == null)
                {
                    if (MyClan != null)
                    {
                        MyClan.Members.TryGetValue(UID, out clanmemb);
                    }
                }
                return clanmemb;
            }
            set
            {
                clanmemb = value;
            }
        }
        public DateTime DeathTime = new DateTime();
        public Instance.Clan MyClan;
        public unsafe uint ClanUID;
        public unsafe ushort ClanRank;

        public int SlayerNormalPercent, SlayerSkillPercent;


        public Role.Instance.Guild.Member MyGuildMember;
        public Role.Instance.Guild MyGuild;
        public uint TargetGuild = 0;

        uint _extbattle;
        public unsafe uint ExtraBattlePower
        {
            get { return _extbattle; }
            set
            {
                _extbattle = value;
            }
        }

        public unsafe Flags.GuildMemberRank GuildRank = Flags.GuildMemberRank.None;
        public unsafe uint GuildID;

        uint guildBP;
        public uint GuildBattlePower
        {
            get
            {
                return guildBP;
            }
            set
            {

                guildBP = value;

                using (var rec = new ServerSockets.RecycledPacket())
                {
                    var stream = rec.GetStream();
                    SendUpdate(stream, guildBP, Game.MsgServer.MsgUpdate.DataType.GuildBattlePower);
                }
            }
        }


        uint _mentorBp;
        private uint MentorBp
        {
            get { return _mentorBp; }
            set
            {
                ExtraBattlePower -= _mentorBp;
                ExtraBattlePower += value;
                _mentorBp = value;
            }
        }
        public unsafe void SetMentorBattlePowers(uint val, uint mentorPotency)
        {

            MentorBp = val;
            using (var rec = new ServerSockets.RecycledPacket())
            {
                var stream = rec.GetStream();

                Game.MsgServer.MsgUpdate upd = new Game.MsgServer.MsgUpdate(stream, UID, 2);
                stream = upd.Append(stream, Game.MsgServer.MsgUpdate.DataType.ExtraBattlePower, val);
                stream = upd.Append(stream, Game.MsgServer.MsgUpdate.DataType.ExtraBattlePower, mentorPotency);
                stream = upd.GetArray(stream);
                Owner.Send(stream);
            }
        }

        public uint targetTrade = 0;

        public Role.Instance.Associate.MyAsociats MyMentor = null;
        public Role.Instance.Associate.MyAsociats Associate;


        public uint TradePartner = 0;
        public uint TargetFriend = 0;
        public Role.Instance.Nobility Nobility;
        Role.Instance.Nobility.NobilityRank _NobilityRank;
        public Role.Instance.Nobility.NobilityRank NobilityRank
        {
            get { return _NobilityRank; }
            set
            {
                _NobilityRank = value;
                if (MyGuild != null && MyGuildMember != null)
                    MyGuildMember.NobilityRank = (uint)_NobilityRank;
                if (UnionMemeber != null)
                    UnionMemeber.NobilityRank = _NobilityRank;
            }
        }

        public Instance.Flowers Flowers;
        public unsafe int FlowerRank;
        public bool OnFairy = false;
        public unsafe Game.MsgServer.MsgTransformFairy FairySpawn;


        public Instance.Chi MyChi;
        public List<Role.Instance.Chi.ChiPower> ChiPowers;
        public ushort ActiveDance = 0;
        public Client.GameClient ObjInteraction;

        uint _KingDomExploits;
        public uint KingDomExploits
        {
            get { return _KingDomExploits; }
            set
            {
                _KingDomExploits = value;
                if (InUnion)
                    UnionMemeber.Exploits = value;
                UpdateExploitsRank();

            }
        }
        public void UpdateExploitsRank()
        {

            if (KingDomExploits < 200)
                return;

            if (KingDomExploits >= 23000)
                SetExploitsRank(Role.Flags.ExploitsRank.GeneralinChief);
            else if (KingDomExploits >= 15000)
                SetExploitsRank(Role.Flags.ExploitsRank.FlyingCavalryGeneral);
            else if (KingDomExploits >= 10000)
                SetExploitsRank(Role.Flags.ExploitsRank.ChariotsandCavalryGeneral);
            else if (KingDomExploits >= 7500)
                SetExploitsRank(Role.Flags.ExploitsRank.ChiefofStaff);
            else if (KingDomExploits >= 6000)
                SetExploitsRank(Role.Flags.ExploitsRank.General);
            else if (KingDomExploits >= 4700)
                SetExploitsRank(Role.Flags.ExploitsRank.AssistantGeneral);
            else if (KingDomExploits >= 3700)
                SetExploitsRank(Role.Flags.ExploitsRank.DeputyGeneral);
            else if (KingDomExploits >= 2800)
                SetExploitsRank(Role.Flags.ExploitsRank.MasterSergeant);
            else if (KingDomExploits >= 2100)
                SetExploitsRank(Role.Flags.ExploitsRank.StaffSergeant);
            else if (KingDomExploits >= 1500)
                SetExploitsRank(Role.Flags.ExploitsRank.Sergeant);
            else if (KingDomExploits >= 1000)
                SetExploitsRank(Role.Flags.ExploitsRank.Centurion);
            else if (KingDomExploits >= 500)
                SetExploitsRank(Role.Flags.ExploitsRank.Decurion);
            else if (KingDomExploits >= 200)
                SetExploitsRank(Role.Flags.ExploitsRank.Corporal);
        }
        public Role.Flags.ExploitsRank ExploitsRank = 0;
        public void SetExploitsRank(Role.Flags.ExploitsRank rank)
        {
            ExploitsRank = rank;
            using (var rec = new ServerSockets.RecycledPacket())
            {
                var stream = rec.GetStream();
                SendUpdate(stream, (long)rank, MsgUpdate.DataType.ExploitsRank, true);
            }
        }

        public unsafe Game.MsgServer.InteractQuery InteractionEffect = default(Game.MsgServer.InteractQuery);
        public bool OnInteractionEffect = false;

        public Instance.SubClass SubClass;
        public unsafe uint SubClassHasPoints;
        public unsafe Database.DBLevExp.Sort ActiveSublass;

        public uint ToxicLevel = 0;
        public bool ContainReflect { get { return Database.AtributesStatus.IsWarrior(SecoundeClass); } }
        public bool BlackSpot = false;
        public DateTime Stamp_BlackSpot = new DateTime();

        public byte UseStamina = 0;
        public DateTime Protect = new DateTime();
        private DateTime ProtectedJumpAttack = new DateTime();
        internal void ProtectAttack(int StampMiliSecounds)
        {
            Protect = DateTime.Now.AddMilliseconds(StampMiliSecounds);
        }
        internal void ProtectJumpAttack(int Secounds)
        {
            ProtectedJumpAttack = DateTime.Now.AddSeconds(Secounds);
        }
        internal bool AllowAttack()
        {
            return DateTime.Now >= Protect && DateTime.Now >= ProtectedJumpAttack;
        }
        public uint ShieldBlockDamage = 0;

        internal void CheckAura()
        {
            if (UseAura != Game.MsgServer.MsgUpdate.Flags.Normal)
            {
                IncreaseStatusAura(UseAura, Aura);
            }
        }

        public Game.MsgServer.MsgUpdate.Flags UseAura = Game.MsgServer.MsgUpdate.Flags.Normal;
        public Database.MagicType.Magic Aura;
        private int AuraTimer = 0;

        internal unsafe bool AddAura(Game.MsgServer.MsgUpdate.Flags flag, Database.MagicType.Magic new_aura, int Timer)
        {
            if (flag == UseAura)
            {
                RemoveFlag(UseAura);
                DecreaseStatusAura(UseAura);
                UseAura = Game.MsgServer.MsgUpdate.Flags.Normal;
                return false;
            }
            AuraTimer = Timer;
            if (UseAura != Game.MsgServer.MsgUpdate.Flags.Normal)
            {
                RemoveFlag(UseAura);
                DecreaseStatusAura(UseAura);
                UseAura = Game.MsgServer.MsgUpdate.Flags.Normal;
            }
            UseAura = flag;
            Aura = new_aura;
            IncreaseStatusAura(flag, new_aura);
            AddFlag(flag, Timer, true, 0);

            Game.MsgServer.MsgFlagIcon.ShowIcon icon = MsgFlagIcon.ShowIcon.EarthAura;

            if (flag == Game.MsgServer.MsgUpdate.Flags.FendAura) icon = Game.MsgServer.MsgFlagIcon.ShowIcon.FendAura;
            else if (flag == Game.MsgServer.MsgUpdate.Flags.FendAura) icon = Game.MsgServer.MsgFlagIcon.ShowIcon.FendAura;
            else if (flag == Game.MsgServer.MsgUpdate.Flags.TyrantAura) icon = Game.MsgServer.MsgFlagIcon.ShowIcon.TyrantAura;
            else if (flag == Game.MsgServer.MsgUpdate.Flags.MetalAura) icon = Game.MsgServer.MsgFlagIcon.ShowIcon.MetalAura;
            else if (flag == Game.MsgServer.MsgUpdate.Flags.WoodAura) icon = Game.MsgServer.MsgFlagIcon.ShowIcon.WoodAura;
            else if (flag == Game.MsgServer.MsgUpdate.Flags.WaterAura) icon = Game.MsgServer.MsgFlagIcon.ShowIcon.WaterAura;
            else if (flag == Game.MsgServer.MsgUpdate.Flags.FireAura) icon = Game.MsgServer.MsgFlagIcon.ShowIcon.FireAura;
            else if (flag == Game.MsgServer.MsgUpdate.Flags.EartAura) icon = Game.MsgServer.MsgFlagIcon.ShowIcon.EarthAura;
            else if (flag == Game.MsgServer.MsgUpdate.Flags.NobleSpirit) icon = Game.MsgServer.MsgFlagIcon.ShowIcon.NobleSpirit;
            else if (flag == Game.MsgServer.MsgUpdate.Flags.WackeSpirit) icon = Game.MsgServer.MsgFlagIcon.ShowIcon.WackeSpirit;
            using (var rec = new ServerSockets.RecycledPacket())
            {
                var stream = rec.GetStream();

                Owner.Send(stream.FlagIconCreate(UID, icon, new_aura.Level, (uint)new_aura.Damage));
            }

            return true;
        }

        private void DecreaseStatusAura(Game.MsgServer.MsgUpdate.Flags flag)
        {
            if (flag == Game.MsgServer.MsgUpdate.Flags.FendAura)
                Owner.Status.Immunity -= (uint)(Aura.Damage * 100);
            else if (flag == Game.MsgServer.MsgUpdate.Flags.FendAura)
                Owner.Status.Immunity -= (uint)(Aura.Damage * 100);
            else if (flag == Game.MsgServer.MsgUpdate.Flags.TyrantAura)
                Owner.Status.CriticalStrike -= (uint)(Aura.Damage * 100);
            else if (flag == Game.MsgServer.MsgUpdate.Flags.MetalAura)
                Owner.Status.MetalResistance -= (uint)Aura.Damage;
            else if (flag == Game.MsgServer.MsgUpdate.Flags.WoodAura)
                Owner.Status.WoodResistance -= (uint)Aura.Damage;
            else if (flag == Game.MsgServer.MsgUpdate.Flags.WaterAura)
                Owner.Status.WaterResistance -= (uint)Aura.Damage;
            else if (flag == Game.MsgServer.MsgUpdate.Flags.FireAura)
                Owner.Status.FireResistance -= (uint)Aura.Damage;
            else if (flag == Game.MsgServer.MsgUpdate.Flags.EartAura)
                Owner.Status.EarthResistance -= (uint)Aura.Damage;
            else if (flag == Game.MsgServer.MsgUpdate.Flags.NobleSpirit)
            {
                uint Rate = (uint)Aura.Damage % 10 * 100;
                Owner.Status.Penetration -= (uint)Rate;
                Owner.Status.MagicDefence -= (uint)Aura.Damage / 100;
            }
            else if (flag == Game.MsgServer.MsgUpdate.Flags.WackeSpirit)
            {
                Owner.Status.DodgeRate -= (uint)Aura.Damage;
            }

        }
        private void IncreaseStatusAura(Game.MsgServer.MsgUpdate.Flags flag, Database.MagicType.Magic new_aura)
        {
            if (flag == Game.MsgServer.MsgUpdate.Flags.FendAura)
                Owner.Status.Immunity += (uint)(new_aura.Damage * 100);
            else if (flag == Game.MsgServer.MsgUpdate.Flags.FendAura)
                Owner.Status.Immunity += (uint)(new_aura.Damage * 100);
            else if (flag == Game.MsgServer.MsgUpdate.Flags.TyrantAura)
                Owner.Status.CriticalStrike += (uint)(new_aura.Damage * 100);
            else if (flag == Game.MsgServer.MsgUpdate.Flags.MetalAura)
                Owner.Status.MetalResistance += (uint)new_aura.Damage;
            else if (flag == Game.MsgServer.MsgUpdate.Flags.WoodAura)
                Owner.Status.WoodResistance += (uint)new_aura.Damage;
            else if (flag == Game.MsgServer.MsgUpdate.Flags.WaterAura)
                Owner.Status.WaterResistance += (uint)new_aura.Damage;
            else if (flag == Game.MsgServer.MsgUpdate.Flags.FireAura)
                Owner.Status.FireResistance += (uint)new_aura.Damage;
            else if (flag == Game.MsgServer.MsgUpdate.Flags.EartAura)
                Owner.Status.EarthResistance += (uint)new_aura.Damage;
            else if (flag == Game.MsgServer.MsgUpdate.Flags.NobleSpirit)
            {
                uint Rate = (uint)Aura.Damage % 10 * 100;
                Owner.Status.Penetration += (uint)Rate;
                Owner.Status.MagicDefence += (uint)Aura.Damage / 100;

            }
            else if (flag == Game.MsgServer.MsgUpdate.Flags.WackeSpirit)
            {
                Owner.Status.DodgeRate += (uint)new_aura.Damage;


            }
        }
        public uint ResinanceRuneOne = 0;
        public uint ResinanceRuneTwo = 0;
        public uint ResinanceRunethree = 0;
        public uint ResinanceRunefour = 0;
        public uint ResinanceRunefive = 0;
        public uint SpouseUID = 0;
        public SpellData GetSpellByID(List<SpellData> spellList, ushort spellID)
        {
            return spellList.Find(s => s.SpellID == (ushort)spellID);
        }
        public class SpellData
        {
            public DateTime SpellNow;
            public int SpellID { get; set; }
        }
        public List<SpellData> spellList = new List<SpellData>();
        public DateTime AttackStamp = new DateTime();
        public DateTime SpellAttackStamp = new DateTime();
        public DateTime SpellNow = DateTime.Now;
        public bool OnTransform { get { return TransformationID != 0; } }
        public ClientTransform TransformInfo = null;
        public byte PoisonLevel = 0;
        public uint FlameOfDestructionDg = 0;
        public uint FlameOfDestructionDamage = 0;
        public uint DrainsHP = 0;
        public uint WhirlSigilDg = 0;
        public uint DragonShiftCount = 0;
        public uint StarChainFireDg = 0;
        public uint KylinSigilGate = 0;
        public uint FutilityDg = 0;
        public byte PoisonLevehHu = 0;
        public int BleedDamage = 0;
        public bool ActivateCounterKill = false;
        internal uint TaoHeavenState;

        public uint DunePoints = 0;
        public Action<Client.GameClient> MessageOK;
        public Action<Client.GameClient> MessageCancel;
        public DateTime StartMessageBox = new DateTime();
        public unsafe void MessageBox(string text, Action<Client.GameClient> msg_ok = null, Action<Client.GameClient> msg_cancel = null, int secounds = 0, Game.MsgServer.MsgStaticMessage.Messages messaj = Game.MsgServer.MsgStaticMessage.Messages.None)
        {
            if (Owner.Player.ContainFlag((MsgUpdate.Flags)438))
                return;
            if (Owner.Player.ContainFlag(MsgUpdate.Flags.SoulShackle))
                return;
            if (life)
                return;
            if (Owner.Player.Map == 3053 || Owner.Player.Map == 1860 || Owner.Player.Map == 10001 && !Owner.Player.CanOut)
                return;
            if (
    Owner.Player.Map == 5061 ||
    Owner.Player.Map == 5062 ||
    Owner.Player.Map == 5063 ||
    Owner.Player.Map == 5064 ||
    Owner.Player.Map == 5065 ||
    Owner.Player.Map == 5066 ||
    Owner.Player.Map == 5051 ||
    Owner.Player.Map == 5052 ||
    Owner.Player.Map == 5053 ||
    Owner.Player.Map == 5054 ||
    Owner.Player.Map == 5055 ||
    Owner.Player.Map == 5056 ||
    Owner.Player.Map == 5057 ||
    Owner.Player.Map == 5058 || Owner.Player.Map == 22341 || Owner.Player.Map == 10741 || Owner.Player.Map == 3030 || Owner.Player.Map == 22340 || Owner.Player.Map == 26400 || Owner.Player.Map == 50016 || Owner.Player.Map == 50100 || Owner.Player.Map == 50101 || Owner.Player.Map == 50102 || Owner.Player.Map == 50020 || Owner.Player.Map == 50021 || Owner.Player.Map == 50018 || Owner.Player.Map == 50019 || Owner.Player.Map == 50021 || Owner.Player.Map == 50104 || Owner.Player.Map == 50105 || Owner.Player.Map == 1518 || Owner.Player.Map == 1508 || Owner.Player.Map == 50017 || Owner.Player.Map == 3581 || Owner.Player.Map == 6570 || Owner.Player.Map == 6521 || Owner.Player.Map == 3071 || Owner.Player.Map == 2515 || Owner.Player.Map == 10134 || Owner.Player.Map == 10133 || Owner.Player.Map == 6526 || Owner.Player.Map == 1485 || Owner.Player.Map == 1484 || Owner.Player.Map == 1483 || Owner.Player.Map == 1486 || Owner.Player.Map == 5661 || Owner.Player.Map == 9855 || Owner.Player.Map == 1487 || Owner.Player.Map == 1138 || Owner.Player.Map == 1038 || Owner.Player.Map == 22389 || Owner.Player.Map == 22382 || Owner.Player.Map == 22385 || Owner.Player.Map == 22384 || Owner.Player.Map == 22387 || Owner.Player.Map == 22386 || Owner.Player.Map == 26702 || Owner.Player.Map == 26701 || Owner.Player.Map == 26700 || Owner.Player.Map == 22381 || Owner.Player.Map == 22380 || Owner.Player.Map == 22383 || Owner.Player.Map == 22388 || Owner.Player.Map == 22330 || Owner.Player.Map == 12816)
                return;
            if (!OnMyOwnServer)
                return;

            if (Map == 5040)
                return;
            if (Owner.InQualifier())
                return;
            if (Owner.IsWatching())
                return;
            if (!CompleteLogin)
                return;
            if (global::Qahr.Mobile.Phone.MobileEvents.Divert(Owner, text, msg_ok, secounds))
                return;// QahrMobile: event invitations become a chat line + @join on the mobile (Data\mobile_events.txt)
            using (var rec = new ServerSockets.RecycledPacket())
            {
                var stream = rec.GetStream();

                MessageOK = msg_ok;
                MessageCancel = msg_cancel;
                Game.MsgNpc.Dialog dialog = new Game.MsgNpc.Dialog(Owner, stream);
                dialog.CreateMessageBox(text).FinalizeDialog(true);
                StartMessageBox = DateTime.Now.AddHours(24);
                if (secounds != 0)
                {
                    StartMessageBox = DateTime.Now.AddSeconds(secounds);
                    if (messaj != Game.MsgServer.MsgStaticMessage.Messages.None)
                    {
                        Owner.Send(stream.StaticMessageCreate(messaj, MsgStaticMessage.Action.Append, (uint)secounds));
                    }

                }
            }
        }

        public void RemoveBuffersMovements(ServerSockets.Packet stream)
        {
            InUseIntensify = false;
            RemoveFlag(Game.MsgServer.MsgUpdate.Flags.Focused);
            if (ContainFlag(Game.MsgServer.MsgUpdate.Flags.WeepStorm))
            {
                RemoveFlag(MsgUpdate.Flags.WeepStorm);
            }
            //if (ContainFlag(Game.MsgServer.MsgUpdate.Flags.CTF_Flag)) // كود بيخلي الفلاج لما بتاخدو في الكبشر فلاج يختفي
            //{
            //    RemoveFlag(MsgUpdate.Flags.CTF_Flag);
            //}
            if (Owner.Fake)
            {
                if (ContainFlag(Game.MsgServer.MsgUpdate.Flags.FlashingName))
                {
                    RemoveFlag(MsgUpdate.Flags.FlashingName);
                }
            }
            RemoveFlag(Game.MsgServer.MsgUpdate.Flags.Praying);
            RemoveFlag(Game.MsgServer.MsgUpdate.Flags.CastPray);
            if (ContainFlag(Game.MsgServer.MsgUpdate.Flags.MagicDefender))
            {
                RemoveFlag(Game.MsgServer.MsgUpdate.Flags.MagicDefender);
                SendUpdate(stream, Game.MsgServer.MsgUpdate.Flags.MagicDefender, 0
   , 0, 0, Game.MsgServer.MsgUpdate.DataType.AzureShield, true);
            }
        }

        public bool InUseIntensify = false;
        public DateTime IntensifyStamp = new DateTime();
        public int IntensifyDamage = 0;

        public bool InUseWeepStorm = false;
        public DateTime WeepStormStamp = new DateTime();
        public int WeepStormDamage = 0;
        public int DeadwoodIncFinal = 0;
        public byte AreaOccupier = 0;
        public int BattlePower
        {
            get
            {
                int val = (int)((Database.ProfessionTable.Benefits.ContainsKey(Class) ? Database.ProfessionTable.Benefits[Class].BattlePower : 0) + Level + Reborn * 5 + Owner.Equipment.BattlePower + (byte)NobilityRank + ExtraBattlePower + GuildBattlePower);

                if (Owner.GodDmgMode)
                    return 1000;

                return Math.Min(522, val);
            }
        }
        public int RealBattlePower
        {
            get
            {
                int val = (int)(Level + Reborn * 5 + Owner.Equipment.BattlePower + (byte)NobilityRank);

                if (Owner.GodDmgMode)
                    return val + 1000;

                return val;
            }
        }
        ushort azuredef;
        public byte AzureShieldLevel;
        public ushort AzureShieldDefence
        {
            get { return azuredef; }
            set
            {
                azuredef = value;
                using (var rec = new ServerSockets.RecycledPacket())
                {
                    var stream = rec.GetStream();
                    SendUpdate(stream, Game.MsgServer.MsgUpdate.Flags.AzureShield, 60
                        , value, AzureShieldLevel, Game.MsgServer.MsgUpdate.DataType.AzureShield, true);
                }
            }
        }

        public DateTime XPListStamp = new DateTime(), StaminaStamp = new DateTime();
        public ushort Stamina = 0;
        public ushort HitRateFantasy = 0;
        public byte WaveBreaKLife = 0;
        public ushort LowPhyicalFantasy = 0;
        public bool EagleEyeCountDown = false;
        public DateTime EagleEyeStamp;
        //public bool PtsGuildwarFinalize(ServerSockets.Packet stream, bool ComboKillUpdate = false)
        //{
        //    if (this.MyGuild == null || this.MyGuildMember == null || MsgSchedules.NEWGUILDWAR == null || MsgSchedules.NEWGUILDWAR.Proces != ProcesType.Alive || this.Map != 1038U)
        //        return false;
        //    if (MsgSchedules.NEWGUILDWAR.VlmScoreInfoList.ContainsKey(this.UID))
        //        MsgSchedules.NEWGUILDWAR.VlmScoreInfoList[this.UID].ContributionPts = (int)this.MyGuildMember.GuildWarPoints;
        //    if (ComboKillUpdate)
        //        this.Send(stream.CreateMsgVlmScoreInfo(new MsgVlmScoreInfo.MsgVlmScoreInfoProto()
        //        {
        //            Type = 10U,
        //            ComboKillInfo = new MsgVlmScoreInfo.ComboKill()
        //            {
        //                ComboKills = 1U,
        //                Kills = 1U
        //            }
        //        }));
        //    return true;
        //}
        //public bool MemberExploitsFinalize(ServerSockets.Packet stream, bool ComboKillUpdate = false)
        //{
        //    if (this.MyGuild == null || this.MyGuildMember == null || MsgSchedules.CaptureTheFlag == null || MsgSchedules.CaptureTheFlag.Proces != ProcesType.Alive || this.Map != 2057U)
        //        return false;
        //    if (MsgSchedules.CaptureTheFlag.VlmScoreInfoList.ContainsKey(this.UID))
        //        MsgSchedules.CaptureTheFlag.VlmScoreInfoList[this.UID].ContributionPts = (int)this.MyGuildMember.CTF_Exploits;
        //    if (ComboKillUpdate)
        //        this.Send(stream.CreateMsgVlmScoreInfo(new MsgVlmScoreInfo.MsgVlmScoreInfoProto()
        //        {
        //            Type = 3U,
        //            ComboKillInfo = new MsgVlmScoreInfo.ComboKill()
        //            {
        //                ComboKills = 1U,
        //                Kills = 1U
        //            }
        //        }));
        //    return true;
        //}

        public StatusFlagsBigVector32 BitVector;
        public void AddSpellFlag(Game.MsgServer.MsgUpdate.Flags Flag, int Secounds, bool RemoveOnDead, int StampSecounds = 0)
        {
            if (BitVector.ContainFlag((int)Flag))
                BitVector.TryRemove((int)Flag);
            AddFlag(Flag, Secounds, RemoveOnDead, StampSecounds);
        }
        public bool AddFlag(Game.MsgServer.MsgUpdate.Flags Flag, int Secounds, bool RemoveOnDead, int StampSecounds = 0, uint showamount = 0, uint amount = 0)
        {
            switch (Flag)
            {
                case MsgUpdate.Flags.Cyclone:
                case MsgUpdate.Flags.Oblivion:
                case MsgUpdate.Flags.Superman:
                case MsgUpdate.Flags.FatalStrike:
                case MsgUpdate.Flags.ShurikenVortex:
                case MsgUpdate.Flags.ChaintBolt:
                case MsgUpdate.Flags.BlackbeardsRage:
                case MsgUpdate.Flags.CannonBarrage:
                case MsgUpdate.Flags.BladeFlurry:
                case MsgUpdate.Flags.SuperCyclone:
                case MsgUpdate.Flags.DragonCyclone:
                case MsgUpdate.Flags.Omnipotence:
                case MsgUpdate.Flags.ThunderRampage:
                    {
                        Owner.Rune.IsEquipped("XPBooster", ref XPBooster);
                        Owner.Rune.IsEquipped("XP Devourer", ref XPBooster);
                        XPKiller = 0;
                        break;
                    }
            }

            if (!BitVector.ContainFlag((int)Flag))
            {

                BitVector.TryAdd((int)Flag, Secounds, RemoveOnDead, StampSecounds);

                UpdateFlagOffset();
                int asd = (int)Flag;
                if ((int)Flag >= 52 && (int)Flag <= 60)
                {
                    using (var rec = new ServerSockets.RecycledPacket())
                    {
                        var stream = rec.GetStream();

                        View.SendView(stream.GameUpdateCreate(UID, (Game.MsgServer.MsgGameUpdate.DataType)Flag, true, showamount, (uint)Secounds, amount), true);

                    }
                }
                return true;
            }
            return false;
        }
        public bool RemoveFlag(Game.MsgServer.MsgUpdate.Flags Flag)
        {
            if (BitVector.ContainFlag((int)Flag))
            {
                BitVector.TryRemove((int)Flag);
                if (Flag == MsgUpdate.Flags.XPList)
                {
                    using (var recycledPacket = new ServerSockets.RecycledPacket())
                    {
                        var streamm = recycledPacket.GetStream();
                        {
                            SendUpdate(streamm, 0, MsgUpdate.DataType.XPList);
                        }
                    }
                }
                UpdateFlagOffset();
                if (Flag == MsgUpdate.Flags.Oblivion)
                {
                    using (var rec = new ServerSockets.RecycledPacket())
                    {
                        var stream = rec.GetStream();
                        Owner.IncreaseExperience(stream, Owner.ExpOblivion);
                    }
                    Owner.ExpOblivion = 0;
                }

                if (Flag == MsgUpdate.Flags.Focused && FocusClientSpell != null)
                {
                    using (var recycledPacket = new ServerSockets.RecycledPacket())
                    {
                        var streamm = recycledPacket.GetStream();
                        {
                            Owner.Send(streamm.ActionCreate(new ActionQuery() { ObjId = UID, TargetUID = FocusClientSpell.ID, Timestamp = (uint)Time32.timeGetTime().GetHashCode(), Type = 103 }));
                        }
                    }
                }
                if (Flag == MsgUpdate.Flags.ShieldBlock)
                {
                    Owner.Equipment.OnDequeue();
                }
                if ((int)Flag >= 52 && (int)Flag <= 60)
                {
                    using (var rec = new ServerSockets.RecycledPacket())
                    {
                        var stream = rec.GetStream();

                        Owner.Send(stream.GameUpdateCreate(UID, (Game.MsgServer.MsgGameUpdate.DataType)Flag, false, 0, 0, 0));

                    }
                }
                return true;
            }
            return false;
        }
        public bool UpdateFlag(Game.MsgServer.MsgUpdate.Flags Flag, int Secounds, bool SetNewTimer, int MaxTime)
        {
            return BitVector.UpdateFlag((int)Flag, Secounds, SetNewTimer, MaxTime);
        }
        public void ClearFlags()
        {
            if (BitVector.Contain((int)MsgUpdate.Flags.XPList))
            {
                using (var recycledPacket = new ServerSockets.RecycledPacket())
                {
                    var streamm = recycledPacket.GetStream();
                    {
                        SendUpdate(streamm, 0, MsgUpdate.DataType.XPList);

                    }
                }
            }
            BitVector.GetClear();
            UpdateFlagOffset();
            Owner.Equipment.QueryEquipment(Owner.Equipment.Alternante);
        }
        public DateTime RestoringHPStamp, RestoringStaminaStamp;
        public bool ActiveChaoticDance = false;
        public bool ActiveSuperSpeedGun = false;
        public uint IncreasingHP;
        public uint RestoringHP;
        public uint IncreaseasStaminaPercent;

        public DateTime PreviousFall;
        public DateTime FallStamp = new DateTime();
        public bool ContainFlag(Game.MsgServer.MsgUpdate.Flags Flag)
        {
            return BitVector.ContainFlag((int)Flag);
        }
        public bool CheckInvokeFlag(Game.MsgServer.MsgUpdate.Flags Flag, DateTime timer32)
        {
            return BitVector.CheckInvoke((int)Flag, timer32);
        }
        public unsafe void UpdateFlagOffset()
        {
            SendUpdate(BitVector.bits, Game.MsgServer.MsgUpdate.DataType.StatusFlag, true);
        }
        public unsafe void SendUpdateHP()
        {
            using (var rec = new ServerSockets.RecycledPacket())
            {
                var MyStream = rec.GetStream();
                Game.MsgServer.MsgUpdate Upd = new Game.MsgServer.MsgUpdate(MyStream, UID, 2);
                MyStream = Upd.Append(MyStream, Game.MsgServer.MsgUpdate.DataType.MaxHitpoints, Owner.Status.MaxHitpoints);
                MyStream = Upd.Append(MyStream, Game.MsgServer.MsgUpdate.DataType.Hitpoints, HitPoints);
                MyStream = Upd.GetArray(MyStream);
                Owner.Send(MyStream);
            }
        }
        public ushort Dead_X;
        public ushort Dead_Y;

        public bool GetPkPkPoints = false;
        public bool CompleteLogin = false;

        public DateTime GhostStamp = new DateTime();
        public uint deathtime = 20;
        public unsafe void IsDead(Role.Player user)
        {
            GhostStamp = DateTime.Now.AddMilliseconds(1000);
            Dead_X = user.X;
            Dead_Y = user.Y;
            DeadStamp = DateTime.Now;
            ClearFlags();
            HitPoints = 0;
            AddFlag(Game.MsgServer.MsgUpdate.Flags.Dead, StatusFlagsBigVector32.PermanentFlag, true);
            using (var rec = new ServerSockets.RecycledPacket())
            {
                var stream = rec.GetStream();

                InteractQuery action = new InteractQuery()
                {
                    UID = 0,
                    X = user.X,
                    Y = user.Y,
                    AtkType = (ushort)MsgAttackPacket.AttackID.Death,
                    OpponentUID = UID
                };
                View.SendView(stream.InteractionCreate(action), true);

            }

        }
        public unsafe void Dead(Role.Player killer, ushort DeadX, ushort DeadY, uint KillerUID)
        {



            #region Thundercloud
            foreach (var obj in Owner.Map.View.GetAllMapRoles(Role.MapObjectType.Monster))
            {
                var pet = obj as Game.MsgMonster.MonsterRole;
                if (pet.Name == "Thundercloud" && pet.OwnerFloor.Player.UID == UID)
                {
                    ActionQuery action;

                    action = new ActionQuery()
                    {
                        ObjId = pet.FloorPacket.m_UID,
                        Type = ActionType.RemoveEntity
                    };

                    using (var rec = new ServerSockets.RecycledPacket())
                    {
                        var stream = rec.GetStream();
                        pet.View.SendScreen(stream.ActionCreate(action), pet.GMap);
                    }
                    pet.GMap.View.LeaveMap<Role.IMapObj>(pet);
                    pet.HitPoints = 0;
                    Owner.Map.SetMonsterOnTile(pet.X, pet.Y, false);
                }
            }
            #endregion
            if ((ContainFlag(MsgUpdate.Flags.ThunderRampage)) && Database.AtributesStatus.IsThunderStriker(Class) && ThunderStrikerUndyingImprinting >= 60)
            {
                ThunderStrikerUndyingImprinting -= 60;
                HitPoints = (int)Owner.Status.MaxHitpoints;
                #region ChainedStorm(Rune Skill)
                if (Owner.Rune.IsEquipped("ChainedStorm") && Owner.MySpells.ClientSpells.ContainsKey((ushort)Role.Flags.SpellID.WindstormBattleaxe))
                {
                    var spell = Pool.Magic[(ushort)Role.Flags.SpellID.WindstormBattleaxe][Owner.MySpells.ClientSpells[(ushort)Role.Flags.SpellID.WindstormBattleaxe].Level];
                    MsgSpellAnimation MsgSpell = new MsgSpellAnimation(UID
                           , 0, X, Y, spell.ID
                           , spell.Level, 0);
                    using (var rec = new ServerSockets.RecycledPacket())
                    {
                        var stream = rec.GetStream();
                        MsgSpell.SetStream(stream);
                        MsgSpell.Send(Owner);
                    }
                    AddSpellFlag(MsgUpdate.Flags.AttackUp, (int)(spell.Duration + 30), true);
                }
                #endregion
                return;
            }
            if (OnTransform && TransformInfo != null)
            {
                TransformInfo.FinishTransform();
            }
            else if (OnTransform)
                TransformationID = 0;
            if (killer != null)
            {
                #region Flag War
                if (killer.Map == 2071)
                {
                    using (var rec = new ServerSockets.RecycledPacket())
                    {
                        var stream = rec.GetStream();
                        // killer.SendString(stream, MsgStringPacket.StringID.Effect, true, "sports_victory");
                    }
                    killer.FlagsPointss += 1;

                    MsgSchedules.SendSysMesage("Player : " + killer.Name + " Have Killed " + Name + " And Win [ 1 Pts ] in Flags War.", MsgMessage.ChatMode.Talk);
                }

                #endregion

                #region Absolution
                if (Owner.Rune.IsEquipped("Absolution") && Owner.MySpells.ClientSpells.ContainsKey((ushort)Role.Flags.SpellID.Absolution))
                {
                    byte Conqueror = 0;
                    int Mohe777 = 0;
                    foreach (IMapObj Obj in this.View.Roles(MapObjectType.Player))
                    {
                        if (killer != null && killer.Owner != null && killer.Owner.Rune != null)
                        {

                            if (killer.Owner.Rune.IsEquipped("Conqueror`sBlade", ref Conqueror) || killer.Owner.Rune.IsEquipped("Tough Steel", ref Conqueror))
                            { // test rune ac msg
                                // killer.Owner.SendSysMesage("Rune Conqueror`sBlade Activated: Absolution effect", MsgMessage.ChatMode.System, MsgMessage.MsgColor.red, false, true);

                                switch (Conqueror)
                                {
                                    case 1: Mohe777 = 20; break;
                                    case 2: Mohe777 = 25; break;
                                    case 3: Mohe777 = 30; break;
                                    case 4: Mohe777 = 35; break;
                                    case 5: Mohe777 = 40; break;
                                    case 6: Mohe777 = 50; break;
                                    case 7: Mohe777 = 60; break;
                                    case 8: Mohe777 = 70; break;
                                    case 9: Mohe777 = 85; break;
                                }
                            }
                        }
                        if (!Role.Core.Rate(Mohe777))
                        {
                            var absSpell = Pool.Magic[(ushort)Role.Flags.SpellID.Absolution][Owner.MySpells.ClientSpells[(ushort)Role.Flags.SpellID.Absolution].Level];
                            if (Time32.Now >= AbsolutionStamp.AddMilliseconds((int)absSpell.ColdTime))
                            {
                                AbsolutionStamp = Time32.Now;
                                AddSpellFlag(MsgUpdate.Flags.Absolution, 20, true);
                                MsgSpellAnimation MsgSpell = new MsgSpellAnimation(UID
                                       , 0, X, Y, absSpell.ID
                                       , absSpell.Level, 0);
                                using (var rec = new ServerSockets.RecycledPacket())
                                {
                                    var stream = rec.GetStream();
                                    MsgSpell.SetStream(stream);
                                    MsgSpell.Send(Owner);
                                    SendUpdate(stream, MsgUpdate.Flags.Absolution, 20, 0, absSpell.Level, MsgUpdate.DataType.Absolution);
                                }
                                HitPoints = (int)Owner.Status.MaxHitpoints;
                                if (killer != null && killer.Owner != null)
                                    killer.Owner.OnAutoAttack = false;
                                return;

                            }
                        }
                    }
                }
                #endregion

                //#region ScorePlayer War
                //if (killer != null && Map == 1040 && MsgSchedules.ScorePlayer.Process == Game.MsgTournaments.ProcesType.Alive)
                //{
                //    MsgSchedules.ScorePlayer.AddKill(killer.UID, killer.Name);
                //    Owner.SendSysMesage("You were killed by [ " + killer.Name + " ] in [ScorePlayer War]! Respawning shortly...", MsgMessage.ChatMode.System, MsgMessage.MsgColor.red);

                //    // Automatic Respawn for Gold Box War
                //    Random R = new Random();
                //    ushort nx = (ushort)R.Next(40, 160);
                //    ushort ny = (ushort)R.Next(40, 160);
                //    Owner.Teleport(nx, ny, 1040);
                //    Revive(new ServerSockets.RecycledPacket().GetStream());
                //    return; // Stop death processing to keep the flow fast
                //}
                //#endregion
                GhostStamp = DateTime.Now.AddMilliseconds(1000);
                Owner.OnAutoAttack = false;

                Owner.SendSysMesage("You are dead.", MsgMessage.ChatMode.System);
                RemoveFlag(MsgUpdate.Flags.BlackName);
                if (ContainFlag(MsgUpdate.Flags.FatalStrike))
                    RemoveFlag(MsgUpdate.Flags.FatalStrike);
                GetPkPkPoints = true;
                if (this.ContainFlag(MsgUpdate.Flags.RedName)
                    || this.ContainFlag(MsgUpdate.Flags.BlackName)
                    || this.ContainFlag(MsgUpdate.Flags.FlashingName))
                    GetPkPkPoints = false;

                using (var rec = new ServerSockets.RecycledPacket())
                {
                    var stream = rec.GetStream();

                    // var clones = MyClones.ToArray();
                    //foreach (var clone in clones)
                    {
                        //clone.RemoveThat(this.Owner);
                    }
                    MyClones.Clear();



                    if (!Program.FreePkMap.Contains(Map))
                    {
                        if (Associate != null && killer != null)
                        {
                            killer.Associate.AddPKExplorer(killer.Owner, this);
                            Associate.AddEnemy(Owner, killer);
                        }
                    }
                    if (killer != null)
                    {
                        if (killer.Owner.OnSoulSpell != 0)
                        {
                            killer.Send(stream.CreateJiangMessage(killer.Owner.OnSoulSpell));
                        }
                        if (killer.InElitePk || killer.InTeamPk)
                        {
                            killer.TournamentKills += 1;
                        }
                        {
                            killer.TournamentKills += 1;
                        }

                        if (Program.MapCounterHits.Contains(Map))
                        {
                            killer.ArenaKills += 1;
                            ArenaDeads += 1;
                        }


                        if (killer.PkMode == Flags.PKMode.Jiang && JiangHuActive != 0)
                        {
                            if (Map == 1002 || Map == 1000 || Map == 1015 || Map == 1020 || Map == 1011)
                            {
                                if (killer.MyJiangHu != null && MyJiangHu != null)
                                    killer.MyJiangHu.Kill(killer.Owner, this.Owner);
                            }
                        }

                    }

                    if (BlackSpot)
                    {
                        BlackSpot = false;

                        View.SendView(stream.BlackspotCreate(false, UID), true);

                    }
                    Dead_X = DeadX;
                    Dead_Y = DeadY;
                    DeadStamp = DateTime.Now;
                    ClearFlags();
                    HitPoints = 0;
                    AddFlag(Game.MsgServer.MsgUpdate.Flags.Dead, StatusFlagsBigVector32.PermanentFlag, true);
                    global::Qahr.Mobile.NewGuildWar.NgwEngine.OnPlayerDead(this, killer); // [NewGuildWar 2026-10-14] rob / keep hero points

                    if (Map == 9855)
                    {
                        Owner.EndQualifier();
                    }
                    RemoveFlag(MsgUpdate.Flags.Freeze);
                    RemoveFlag(MsgUpdate.Flags.TyrantAura);
                    RemoveFlag(MsgUpdate.Flags.FendAura);

                    if (killer != null)
                    {
                        if (killer.Map == 10250)
                            //  Server.SendGlobalPacket(new MsgMessage(killer.Name + " went to Deityland (" + killer.X + "," + killer.Y + ") and defeated " + Name + " What a promising hero! Go ahead and challenge higher-level lengend fighters for precious Runes!", MsgMessage.MsgColor.whitesmoke, MsgMessage.ChatMode.Talk).GetArray(stream));
                            killer.XPCount++;



                        InteractQuery action = new InteractQuery()
                        {
                            UID = killer.UID,
                            X = DeadX,
                            Y = DeadY,
                            AtkType = (ushort)MsgAttackPacket.AttackID.Death,
                            SpellLevel = killer.KillCounter,
                            SpellID = (ushort)(Database.ItemType.IsBow(killer.Owner.Equipment.RightWeapon) ? 5 : 1),
                            OpponentUID = UID,
                        };
                        View.SendView(stream.InteractionCreate(action), true);


                        if (!Program.NoDropItems.Contains(Map) && !Program.FreePkMap.Contains(Map))
                        {
                            if (killer.PkMode != Flags.PKMode.Jiang)
                                //CheckDropItems(killer, stream);

                                CheckPkPoints(killer);
                        }

                    }
                    else
                    {


                        InteractQuery action = new InteractQuery()
                        {
                            UID = KillerUID,
                            X = DeadX,
                            Y = DeadY,
                            AtkType = (ushort)MsgAttackPacket.AttackID.Death,
                            OpponentUID = UID
                        };
                        View.SendView(stream.InteractionCreate(action), true);


                        if (!Program.NoDropItems.Contains(Map) && !Program.FreePkMap.Contains(Map))
                        {
                            //CheckDropItems(killer, stream);
                        }

                    }
                    ///منع الماب
                    if (Owner.Player.Map != 4447
                        || Owner.Player.Map != 4448
                        || Owner.Player.Map != 4449
                        || Owner.Player.Map != 4450
                        || Owner.Player.Map != 4451
                        || Owner.Player.Map != 4452
                        || Owner.Player.Map != 4453
                        || Owner.Player.Map != 4454
                        || Owner.Player.Map != 4455
                        || Owner.Player.Map != 4456
                        || Owner.Player.Map != 4457
                        || Owner.Player.Map != 1038
                        || Owner.Player.Map != 6729
                        || Owner.Player.Map != 6784
                        || Owner.Player.Map != 6546
                        || Owner.Player.Map != 6521
                        || Owner.Player.Map != 12816
                        || Owner.Player.Map != 1505
                        || Owner.Player.Map != 9855
                        || Owner.Player.Map != 10137
                        || Owner.Player.Map != 4456
                        || Owner.Player.Map != 4456
                        || Owner.Player.Map != 4456

                        )
                    {
                        //if (Owner.PerfectionStatus.StraightLife > 0)
                        //{
                        //    if (Role.Core.Rate(Owner.PerfectionStatus.StraightLife))
                        //    {
                        //        View.SendView(stream.MsgRefineEffectCreate(new MsgRefineEffect.RefineEffectProto()
                        //        {
                        //            Effect = MsgRefineEffect.RefineEffects.StraightLife,
                        //            Id = UID,
                        //            dwParam = UID
                        //        }), true);
                        //        this.Revive(stream);

                        //    }
                        //}
                    }

                    if (killer != null && killer.Owner != null)
                    {
                        if (killer.Owner.MySpells.ClientSpells.ContainsKey((ushort)Role.Flags.SpellID.TenFistSword))
                        {
                            var DBSpells = Pool.Magic[(ushort)Role.Flags.SpellID.TenFistSword][killer.Owner.MySpells.ClientSpells[(ushort)Role.Flags.SpellID.TenFistSword].Level];
                            if (Role.Core.Rate(DBSpells.Rate))
                            {
                                AddSpellFlag((MsgUpdate.Flags)DBSpells.Status, (int)DBSpells.Duration, true);
                            }
                        }
                    }
                    if (killer != null && killer.Owner != null && killer.Owner.MySpells.ClientSpells.ContainsKey((ushort)Role.Flags.SpellID.Slayer) && killer.Owner.Rune.IsEquipped("Slayer"))
                    {
                        var spell = Pool.Magic[(ushort)Role.Flags.SpellID.Slayer][killer.Owner.MySpells.ClientSpells[(ushort)Role.Flags.SpellID.Slayer].Level];
                        if (DateTime.Now >= killer.SlayerStamp.AddMilliseconds((int)spell.ColdTime))
                        {
                            killer.SlayerStamp = DateTime.Now;
                            killer.SlayerNormalPercent = spell.DamageOnMonster;
                            killer.SlayerSkillPercent = spell.DamageOnHuman;
                            killer.AddSpellFlag(MsgUpdate.Flags.Slayer, (int)spell.Duration, true);
                            MsgSpellAnimation MsgSpell = new MsgSpellAnimation(killer.UID
                                  , 0, X, Y, spell.ID
                                  , spell.Level, 0);
                            MsgSpell.SetStream(stream);
                            MsgSpell.Send(killer.Owner);
                            killer.SendUpdateSlayer(stream, MsgUpdate.DataType.Slayer, (uint)spell.Duration, false);
                        }
                    }
                    if (Database.AtributesStatus.IsPirate(Class))
                    {
                        if (Owner.MySpells.ClientSpells.ContainsKey((ushort)Role.Flags.SpellID.NeptuneCurse) && Owner.Rune.IsEquipped("Neptune`sCurse"))
                        {
                            var spell = Pool.Magic[(ushort)Role.Flags.SpellID.NeptuneCurse][Owner.MySpells.ClientSpells[(ushort)Role.Flags.SpellID.NeptuneCurse].Level];
                            MsgSpellAnimation MsgSpell = new MsgSpellAnimation(UID
                                   , 0, X, Y, spell.ID
                                   , spell.Level, 0);
                            MsgSpell.SetStream(stream);
                            MsgSpell.Send(Owner);
                            AddSpellFlag(MsgUpdate.Flags.NeptuneCurse, (int)spell.Duration, true);

                            SendUpdate(stream, MsgUpdate.Flags.NeptuneCurse, (uint)spell.Duration, 0, 0, MsgUpdate.DataType.SkillCountdown);
                        }
                    }

                    #region Nirvana Rune New Edit
                    Owner.Player.deathtime = 20;
                    if (Owner.Player.ContainFlag(MsgUpdate.Flags.Dead) || Owner.Player.HitPoints == 0)
                    {
                        byte itemLevels = 0;
                        if (Owner.Rune.IsEquipped("Nirvana", ref itemLevels))
                        {
                            if (itemLevels == 9) Owner.Player.deathtime -= 15;
                            else if (itemLevels == 8) Owner.Player.deathtime -= 12;
                            else Owner.Player.deathtime -= (byte)(4 + itemLevels);
                        }
                        using (var recss = new ServerSockets.RecycledPacket())
                        {
                            var streamss = recss.GetStream();
                            Send(streamss.NirvanaRune(Owner.Player.deathtime));
                        }
                    }
                    #endregion
                }
            }
            else
            {
                Dead_X = DeadX;
                Dead_Y = DeadY;
                DeadStamp = DateTime.Now;
                ClearFlags();
                HitPoints = 0;
                AddFlag(Game.MsgServer.MsgUpdate.Flags.Dead, StatusFlagsBigVector32.PermanentFlag, true);
                using (var rec = new ServerSockets.RecycledPacket())
                {
                    var stream = rec.GetStream();

                    InteractQuery action = new InteractQuery()
                    {
                        UID = KillerUID,
                        X = DeadX,
                        Y = DeadY,
                        AtkType = (ushort)MsgAttackPacket.AttackID.Death,
                        OpponentUID = UID
                    };
                    View.SendView(stream.InteractionCreate(action), true);


                    if (!Program.NoDropItems.Contains(Map) && !Program.FreePkMap.Contains(Map))
                    {
                        //CheckDropItems(killer, stream);
                    }
                }
            }
        }
        public bool life
        {
            get
            {
                return HitPoints < 1;
            }
            set
            {
                throw new NotImplementedException();
            }
        }
        //public void CheckDropItems(Role.Player killer, ServerSockets.Packet stream)
        //{
        //    if (OnMyOwnServer == false)
        //        return;
        //    if (Map == 3935 && Map == 10137)
        //        return;
        //    if (Map == 10250)
        //        return;

        //    byte itemLevel = 0;
        //    if (killer != null && killer.Owner.Rune.IsEquipped("Grabber", ref itemLevel))
        //        itemLevel = (byte)(itemLevel * 5);
        //    if (!Role.Core.Rate(50 + itemLevel)) return;

        //    try
        //    {
        //        ushort x = X;
        //        ushort y = Y;


        //        if (x > 5 && y > 5)
        //        {
        //            var inventoryItems = Owner.Inventory.ClientItems.Values.ToArray();
        //            if (inventoryItems.Length / 4 > 1)
        //            {
        //                uint count = (uint)Pool.GetRandom.Next(1, (int)(inventoryItems.Length / 4));

        //                for (int index = 0; index < count; index++)
        //                {
        //                    try
        //                    {
        //                        if (inventoryItems.Length > index && inventoryItems[index] != null)
        //                        {
        //                            var item = inventoryItems[index];
        //                            if (item.Position == (ushort)Role.Flags.ConquerItem.AleternanteBottle || item.Position == (ushort)Role.Flags.ConquerItem.Bottle)
        //                                continue;
        //                            if (item.Locked == 0 && item.Inscribed == 0 && item.Bound == 0
        //                                && !Database.ItemType.unabletradeitem.Contains(item.ITEM_ID) && !Database.ItemType.IsSash(item.ITEM_ID))
        //                            {

        //                                ushort New_X = (ushort)Pool.GetRandom.Next((ushort)(x - 5), (ushort)(x + 5));
        //                                ushort New_Y = (ushort)Pool.GetRandom.Next((ushort)(y - 5), (ushort)(y + 5));
        //                                if (Owner.Map.AddGroundItem(ref New_X, ref New_Y))
        //                                {
        //                                    DropItem(item, New_X, New_Y, stream);
        //                                }
        //                            }
        //                        }
        //                    }
        //                    catch (Exception e) { MyConsole.WriteLine(e.ToString()); }
        //                }

        //            }
        //        }
        //        if (x > 5 && y > 5)
        //        {
        //            var inventoryItems = Owner.Inventory.ClientItems.Values.ToArray();
        //            if (inventoryItems.Length / 4 > 1)
        //            {
        //                uint count = (uint)Pool.GetRandom.Next(1, (int)(inventoryItems.Length / 4));

        //                for (int index = 0; index < count; index++)
        //                {
        //                    try
        //                    {
        //                        if (inventoryItems.Length > index && inventoryItems[index] != null)
        //                        {
        //                            var item = inventoryItems[index];
        //                            if (item.Position == (ushort)Role.Flags.ConquerItem.AleternanteBottle || item.Position == (ushort)Role.Flags.ConquerItem.Bottle)
        //                                continue;
        //                            if (item.Locked == 0 && item.Inscribed == 0 && item.Bound == 0
        //                                && !Database.ItemType.unabletradeitem.Contains(item.ITEM_ID) && !Database.ItemType.IsSash(item.ITEM_ID))
        //                            {

        //                                ushort New_X = (ushort)Pool.GetRandom.Next((ushort)(x - 5), (ushort)(x + 5));
        //                                ushort New_Y = (ushort)Pool.GetRandom.Next((ushort)(y - 5), (ushort)(y + 5));
        //                                if (Owner.Map.AddGroundItem(ref New_X, ref New_Y))
        //                                {
        //                                    DropItem(item, New_X, New_Y, stream);
        //                                }
        //                            }
        //                        }
        //                    }
        //                    catch (Exception e) { MyConsole.WriteLine(e.ToString()); }
        //                }

        //            }
        //        }
        //        if (PKPoints >= 30 && killer != null)
        //        {
        //            int Count_DropItem = (PKPoints >= 30 && PKPoints <= 99) ? 1 : 2;
        //            var EquipmentArray = Owner.Equipment.CurentEquip.Where(p => p != null &&
        //                 p.Position != (ushort)Role.Flags.ConquerItem.Bottle && p.Position != (ushort)Role.Flags.ConquerItem.AleternanteBottle
        //                 && p.Position != (ushort)Role.Flags.ConquerItem.Garment && p.Position != (ushort)Role.Flags.ConquerItem.AleternanteGarment
        //                 && p.Position != (ushort)Role.Flags.ConquerItem.Steed && p.Position != (ushort)Role.Flags.ConquerItem.SteedMount
        //                 && p.Position != (ushort)Role.Flags.ConquerItem.RightWeaponAccessory && p.Position != (ushort)Role.Flags.ConquerItem.LeftWeaponAccessory).ToArray();

        //            if (EquipmentArray.Length > 0)
        //            {
        //                int trying = 0;
        //                int Dropable = 0;
        //                Dictionary<uint, Game.MsgServer.MsgGameItem> ItemsDrop = new Dictionary<uint, Game.MsgServer.MsgGameItem>();
        //                do
        //                {
        //                    if (trying == 14)
        //                        break;
        //                    byte ArrayPosition = (byte)Pool.GetRandom.Next(0, EquipmentArray.Length);
        //                    var Element = EquipmentArray[ArrayPosition];
        //                    if (!ItemsDrop.ContainsKey(Element.UID))
        //                    {
        //                        ItemsDrop.Add(Element.UID, Element);
        //                        Dropable++;
        //                    }
        //                    trying++;
        //                }
        //                while (Dropable < Count_DropItem);

        //                //remove equip item--------------


        //                foreach (var item in ItemsDrop.Values)
        //                {

        //                    Owner.Send(stream.ItemUsageCreate(MsgItemUsuagePacket.ItemUsuageID.RemoveEquipment, item.UID, item.Position, 0, 0, 0, 0, 0));

        //                    Game.MsgServer.MsgGameItem Remover;
        //                    Owner.Equipment.ClientItems.TryRemove(item.UID, out Remover);

        //                }
        //                //checkGuildBattlePower;
        //                if (MyGuild != null)
        //                    GuildBattlePower = MyGuild.ShareMemberPotency(GuildRank);

        //                //compute status
        //                Owner.Equipment.QueryEquipment(Owner.Equipment.Alternante);

        //                //--------------------------------

        //                //add container Item
        //                foreach (var item in ItemsDrop.Values)
        //                    Owner.Confiscator.AddItem(Owner, killer.Owner, item, stream);
        //                //-----------
        //            }
        //        }
        //    }
        //    catch (Exception e) { MyConsole.WriteLine(e.ToString()); }
        //}
        public void DropItem(Game.MsgServer.MsgGameItem item, ushort x, ushort y, ServerSockets.Packet stream)
        {
            Game.MsgFloorItem.MsgItem DropItem = new Game.MsgFloorItem.MsgItem(item, x, y, Game.MsgFloorItem.MsgItem.ItemType.Item, 0, DynamicID, Map, UID, false, Owner.Map);

            if (Owner.Map.EnqueueItem(DropItem))
            {
                DropItem.SendAll(stream, Game.MsgFloorItem.MsgDropID.Visible);
                Owner.Inventory.Update(item, Role.Instance.AddMode.REMOVE, stream, true);
            }
        }
        private void CheckPkPoints(Role.Player killer)
        {
            if (killer.OnMyOwnServer == true && OnMyOwnServer == false)
                return;
            if (Map == 3935)
                return;
            if (Map == 10137)
                return;
            if (killer.PkMode != Flags.PKMode.Jiang)
            {
                if (!Pool.Constants.FreePkMap.Contains(Map))
                {
                    if (!this.ContainFlag(Game.MsgServer.MsgUpdate.Flags.RedName) && !this.ContainFlag(Game.MsgServer.MsgUpdate.Flags.BlackName))
                    {
                        if (HeavenBlessing > 0)
                        {
                            if (killer.HeavenBlessing > 0)
                            {
                                Owner.LoseDeadExperience(killer.Owner);
                            }
                            else
                            {
                                Owner.SendSysMesage("Your Heaven Blessing takes effect! You lose no EXP!", MsgMessage.ChatMode.System);
                                killer.AddCursed(5 * 60);
                            }
                        }
                        else
                            Owner.LoseDeadExperience(killer.Owner);


                        if (GetPkPkPoints)
                        {
                            if (killer.MyGuild != null)
                            {
                                if (killer.MyGuild.Enemy.ContainsKey(GuildID))
                                {
                                    killer.PKPoints += 3;
                                    if (GuildRank >= Flags.GuildMemberRank.Manager)
                                        killer.MyGuild.SendMessajGuild("The (" + killer.GuildRank.ToString() + ")" + killer.Name + " at killed on (" + GuildRank.ToString() + ")" + Name + " from guild " + MyGuild.GuildName + " in " + Pool.ServerMaps[Map].Name + "", Game.MsgServer.MsgMessage.ChatMode.Guild, Game.MsgServer.MsgMessage.MsgColor.yellow);

                                    return;
                                }
                            }
                            if (killer.MyClan != null)
                            {
                                if (killer.MyClan.Enemy.ContainsKey(ClanUID))
                                {
                                    killer.PKPoints += 3;
                                    return;
                                }
                            }
                            if (killer.Associate.Contain(Role.Instance.Associate.Enemy, UID))
                            {
                                killer.PKPoints += 5;
                                return;
                            }
                            killer.PKPoints += 10;
                        }
                    }
                    else
                    {
                        if (PKPoints > 99)
                        {
                            MyKillerName = killer.Name;
                            MyKillerUID = killer.UID;
                            Owner.Teleport(29, 72, 6000, 0, true);

                            using (var rec = new ServerSockets.RecycledPacket())
                            {
                                var stream = rec.GetStream();
                                Server.SendGlobalPacket(new Game.MsgServer.MsgMessage(Name + " has been captured by " + MyKillerName + " and sent in jail! The world is now safer!", MsgMessage.MsgColor.white, MsgMessage.ChatMode.System).GetArray(stream));
                            }
                        }
                    }

                }

            }
        }
        public unsafe void Revive(ServerSockets.Packet stream)
        {
            ProtectAttack(5 * 1000);// 5 secounds
            ReviveProtection = true;
            HitPoints = (int)Owner.Status.MaxHitpoints;
            ClearFlags();
            TransformationID = 0;


            if (!Owner.MySpells.ClientSpells.ContainsKey((ushort)Role.Flags.SpellID.TwistofWar))
                XPCount = 0;
            SendUpdate(stream, XPCount, MsgUpdate.DataType.XPCircle);

            Stamina = 100;
            SendUpdate(stream, Stamina, MsgUpdate.DataType.Stamina);

            Send(stream.MapStatusCreate(Map, Map, Owner.Map.TypeStatus));
            View.SendView(GetArray(stream, false), false);

            if (Owner.MySpells.ClientSpells.ContainsKey((ushort)Role.Flags.SpellID.Riding))
            {
                #region Runes
                #region QuickMount
                if (Owner.Player.Map == 9855 || Owner.Player.Map == 1005)
                {
                    return;
                }
                byte QuickMountL = 0;
                if (Owner.Rune.IsEquipped("QuickMount", ref QuickMountL))
                {
                    QuickMountL = (byte)(QuickMountL * 10);
                    if (QuickMountL == 50) QuickMountL = 55;
                    else if (QuickMountL == 60) QuickMountL = 65;
                    else if (QuickMountL == 70) QuickMountL = 75;
                    else if (QuickMountL == 80) QuickMountL = 90;
                    else if (QuickMountL == 90) QuickMountL = 100;
                    if (Role.Core.Rate(QuickMountL))
                    {
                        AddFlag(MsgUpdate.Flags.Ride, Role.StatusFlagsBigVector32.PermanentFlag, true);
                        Owner.Vigor = Owner.Status.MaxVigor;
                        Owner.Send(stream.ServerInfoCreate(Owner.Vigor));
                    }
                }
                #endregion
                #endregion
            }
        }
        //public bool UnShackle(ServerSockets.Packet stream, bool ComboKillUpdate = false)
        //{
        //    if (this.MyGuild == null || this.MyGuildMember == null || MsgSchedules.CaptureTheFlag == null || MsgSchedules.CaptureTheFlag.Proces != ProcesType.Alive || this.Map != 2057U)
        //        return false;
        //    if (MsgSchedules.CaptureTheFlag.VlmScoreInfoList.ContainsKey(this.UID))
        //        MsgSchedules.CaptureTheFlag.VlmScoreInfoList[this.UID].UnShackled = (ulong)this.MyGuildMember.UnShackle;
        //    if (ComboKillUpdate)
        //        this.Send(stream.CreateMsgVlmScoreInfo(new MsgVlmScoreInfo.MsgVlmScoreInfoProto()
        //        {
        //            Type = 3U,
        //            ComboKillInfo = new MsgVlmScoreInfo.ComboKill()
        //            {
        //                ComboKills = 5U,
        //                Kills = 1U
        //            }
        //        }));
        //    return true;
        //}
        //public bool Revive(ServerSockets.Packet stream, bool ComboKillUpdate = false)
        //{
        //    if (this.MyGuild == null || this.MyGuildMember == null || MsgSchedules.CaptureTheFlag == null || MsgSchedules.CaptureTheFlag.Proces != ProcesType.Alive || this.Map != 2057U)
        //        return false;
        //    if (MsgSchedules.CaptureTheFlag.VlmScoreInfoList.ContainsKey(this.UID))
        //        MsgSchedules.CaptureTheFlag.VlmScoreInfoList[this.UID].Revival = (ulong)this.MyGuildMember.Revive;
        //    if (ComboKillUpdate)
        //        this.Send(stream.CreateMsgVlmScoreInfo(new MsgVlmScoreInfo.MsgVlmScoreInfoProto()
        //        {
        //            Type = 3U,
        //            ComboKillInfo = new MsgVlmScoreInfo.ComboKill()
        //            {
        //                ComboKills = 3U,
        //                Kills = 1U
        //            }
        //        }));
        //    return true;
        //}
        public DateTime PkPointsStamp = new DateTime();
        public uint BlessTime = 0;
        public DateTime CastPrayStamp = new DateTime();

        public DateTime CastPrayActionsStamp = new DateTime();

        public Game.MsgServer.MsgUpdate.Flags UseXPSpell;
        public void OpenXpSkill(Game.MsgServer.MsgUpdate.Flags flag, int Timer, int StampExec = 0)
        {
            if (OnAutoHunt)
                return;
            XPCount = 0;
            using (var rec = new ServerSockets.RecycledPacket())
            {
                var stream = rec.GetStream();
                SendUpdate(stream, XPCount, Game.MsgServer.MsgUpdate.DataType.XPCircle);
            }
            Game.MsgServer.MsgUpdate.Flags UseSpell = OnXPSkill();
            if (UseSpell == Game.MsgServer.MsgUpdate.Flags.Normal)
            {
                KillCounter = 0;
                UseXPSpell = flag;
                AddFlag(flag, Timer, true, StampExec);
            }
            else
            {
                if (UseSpell != flag)
                {
                    RemoveFlag(UseSpell);
                    UseXPSpell = flag;
                    AddFlag(flag, Timer, true, StampExec);
                }
                else
                {
                    if (flag == MsgUpdate.Flags.Cyclone || flag == MsgUpdate.Flags.Superman || flag == MsgUpdate.Flags.SuperCyclone)
                        UpdateFlag(flag, Timer, true, 20);
                    else
                        UpdateFlag(flag, Timer, true, 60);
                }
            }
        }
        public Game.MsgServer.MsgUpdate.Flags OnXPSkill()
        {
            if (ContainFlag(Game.MsgServer.MsgUpdate.Flags.Cyclone))
                return Game.MsgServer.MsgUpdate.Flags.Cyclone;
            else if (ContainFlag(Game.MsgServer.MsgUpdate.Flags.Superman))
                return Game.MsgServer.MsgUpdate.Flags.Superman;
            else if (ContainFlag(Game.MsgServer.MsgUpdate.Flags.Oblivion))
                return Game.MsgServer.MsgUpdate.Flags.Oblivion;
            else if (ContainFlag(Game.MsgServer.MsgUpdate.Flags.FatalStrike))
                return Game.MsgServer.MsgUpdate.Flags.FatalStrike;
            else if (ContainFlag(Game.MsgServer.MsgUpdate.Flags.ShurikenVortex))
                return Game.MsgServer.MsgUpdate.Flags.ShurikenVortex;
            else if (ContainFlag(Game.MsgServer.MsgUpdate.Flags.ChaintBolt))
                return Game.MsgServer.MsgUpdate.Flags.ChaintBolt;
            else if (ContainFlag(Game.MsgServer.MsgUpdate.Flags.BlackbeardsRage))
                return Game.MsgServer.MsgUpdate.Flags.BlackbeardsRage;
            else if (ContainFlag(Game.MsgServer.MsgUpdate.Flags.CannonBarrage))
                return Game.MsgServer.MsgUpdate.Flags.CannonBarrage;
            else if (ContainFlag(Game.MsgServer.MsgUpdate.Flags.BladeFlurry))
                return Game.MsgServer.MsgUpdate.Flags.BladeFlurry;
            else if (ContainFlag(Game.MsgServer.MsgUpdate.Flags.SuperCyclone))
                return Game.MsgServer.MsgUpdate.Flags.SuperCyclone;
            else if (ContainFlag(Game.MsgServer.MsgUpdate.Flags.DragonCyclone))
                return Game.MsgServer.MsgUpdate.Flags.DragonCyclone;
            else if (ContainFlag(Game.MsgServer.MsgUpdate.Flags.Omnipotence))
                return Game.MsgServer.MsgUpdate.Flags.Omnipotence;
            else
                return Game.MsgServer.MsgUpdate.Flags.Normal;
        }
        public void UpdateXpSkill()
        {
            if (UseXPSpell == Game.MsgServer.MsgUpdate.Flags.Cyclone
                || UseXPSpell == Game.MsgServer.MsgUpdate.Flags.SuperCyclone
                || UseXPSpell == Game.MsgServer.MsgUpdate.Flags.Superman)
            {
                if (ContainFlag(UseXPSpell))
                    UpdateFlag(UseXPSpell, 1, false, 20);
            }
        }
        public unsafe void SendScrennXPSkill(IMapObj obj)
        {
            if (OnXPSkill() != Game.MsgServer.MsgUpdate.Flags.Normal)
            {


                using (var rec = new ServerSockets.RecycledPacket())
                {
                    var stream = rec.GetStream();

                    InteractQuery action = new InteractQuery()
                    {
                        UID = UID,
                        KilledMonster = 1,
                        X = X,
                        Y = Y,
                        AtkType = (ushort)MsgAttackPacket.AttackID.Death,
                        SpellLevel = KillCounter
                    };
                    obj.Send(stream.InteractionCreate(action));

                }
            }
        }
        public uint Kills = 0;
        public ushort KillCounter;
        ushort _xpc;
        public ushort XPCount
        {
            get
            {
                if (Owner.Player.ContainFlag(MsgUpdate.Flags.NoXp))
                    return 0;
                return _xpc;
            }
            set
            {
                if (Owner.Player.ContainFlag(MsgUpdate.Flags.NoXp))
                    return;
                _xpc = value;

                if (value == 20)
                {
                    Owner.HeroRewards.AddGoal(103);
                }
            }
        }
        public DateTime TeleStamp = new DateTime();
        public DateTime DeadStamp = new DateTime();
        public ushort Avatar;
        public ulong WHMoney;
        public ulong CpsBank;
        public bool IsAsasin = false;
        public bool IsGameMaster()
        {
            return Name.Contains("[GM]") || Name.Contains("[TQ]");
        }
        public bool IsHelpDesk()
        {
            return Name.Contains("[PM]") || Name.Contains("[HD]");
        }
        public Game.MsgServer.ClientAchievement Achievement;
        public DateTime LastWorldMessaj = new DateTime();
        public DateTime LastBroadCast = new DateTime();
        public Flags.PKMode PkMode = Flags.PKMode.Capture;
        public Instance.JiangHu.AttackFlag JiangPkFlag = Instance.JiangHu.AttackFlag.None;
        public Client.GameClient Owner;
        public MapObjectType ObjType { get; set; }
        public RoleView View;
        public Instance.Quests QuestGUI;
        public uint Longineid = 0;
        public uint LongineCount = 0;
        public uint StarChainCount = 0;
        public unsafe void Send(ServerSockets.Packet msg)
        {
            Owner.Send(msg);
        }
        public Player(Client.GameClient _own)
        {
            AllowDynamic = false;
            this.Owner = _own;
            ObjType = MapObjectType.Player;
            View = new RoleView(Owner);
            BitVector = new StatusFlagsBigVector32(32 * 18);
            QuestGUI = new Instance.Quests(this);
            ChiPowers = new List<Instance.Chi.ChiPower>();
            PayNobilitySystem = new DaysNobility(this);
        }
        public int Day = 0;
        public bool FrozenByAdmin = false;
        public bool GodMode = false;

        public unsafe uint UID { get; set; }
        #region Gathering
        public byte Change;
        public uint Points;
        public uint PrizePoints;
        #endregion
        public string UserName = "";
        public unsafe string ClanName = "";
        public string Spouse = "None";
        public string Description = "";
        public ushort Agility;
        public ushort Vitality;
        public ushort Spirit;
        public ushort Strength;
        public ushort Atributes;
        public uint ExchangeNormalAvaliability = 0, ExchangehighAvaliability = 0, AnswerdToday = 0;
        uint _class;
        public unsafe uint Class
        {
            get { return _class; }
            set
            {
                _class = value;
                if (Owner.FullLoading)
                {
                    using (var rec = new ServerSockets.RecycledPacket())
                    {
                        var stream = rec.GetStream();
                        SendUpdate(stream, value, Game.MsgServer.MsgUpdate.DataType.Class);
                    }
                    if (MyGuildMember != null)
                        MyGuildMember.Class = value;
                    //Owner.MyArchives.Close();
                    //Owner.MyArchives.Loading();

                }
            }
        }

        public uint RelicsRate = 0;
        public uint RelicsAllRate = 0;
        public uint RelicResonance = 0;
        public uint RelicResonanceTwoUnlock = 0;

        public byte TwilightDance;
        public byte FirstRebornLevel;
        public byte SecoundeRebornLevel;
        public unsafe byte Reborn;
        public unsafe uint FirstClass;
        public unsafe uint SecoundeClass;
        uint _classexp;
        public unsafe uint ClassExperience
        {
            get { return _classexp; }
            set
            {
                _classexp = value;
                if (Owner.FullLoading)
                {
                    using (var rec = new ServerSockets.RecycledPacket())
                    {
                        var stream = rec.GetStream();
                        SendUpdate(stream, value, Game.MsgServer.MsgUpdate.DataType.ClassExperience);
                    }
                }
            }
        }
        ushort _level;
        public unsafe ushort guildnpc;
        public unsafe ushort trbnpc;
        public unsafe ushort Level
        {
            get { return _level; }
            set
            {
                if (Owner.FullLoading)
                {
                    if (_level != 0 && Reborn == 0)
                    {
                        if (_level < 15 && value >= 15)
                        {
                            using (var rec = new ServerSockets.RecycledPacket())
                            {
                                var stream = rec.GetStream();
                                if (Class == 16000)
                                {
                                    Class = 16001;
                                    SendUpdate(stream, Class, MsgUpdate.DataType.Class);

                                }
                                //Owner.Inventory.Add(stream, 3005160);
                            }
                        }
                        if (_level < 40 && value >= 40)
                        {
                            using (var rec = new ServerSockets.RecycledPacket())
                            {
                                var stream = rec.GetStream();
                                // Owner.Inventory.Add(stream, 3005161);
                                if (Database.AtributesStatus.IsWindWalker(Class))
                                {
                                    if (!QuestGUI.CheckQuest(3797, MsgQuestList.QuestListItem.QuestStatus.Finished))
                                    {
                                        var ActiveQuest = Database.QuestInfo.GetFinishQuest((uint)NpcID.WindwalkerGuard, Class, 3797);
                                        QuestGUI.Accept(ActiveQuest, 0);
                                    }
                                }
                            }
                        }
                        if (_level < 70 && value >= 70)
                        {
                            using (var rec = new ServerSockets.RecycledPacket())
                            {
                                var stream = rec.GetStream();
                                // Owner.Inventory.Add(stream, 3005164);
                            }
                        }
                        if (_level < 100 && value >= 100)
                        {
                            using (var rec = new ServerSockets.RecycledPacket())
                            {
                                var stream = rec.GetStream();
                                //  Owner.Inventory.Add(stream, 3005167);
                            }
                        }
                    }
                    if (Database.AtributesStatus.IsTrojan(Class))
                    {
                        if (_level < 15 && value >= 15)
                        {
                            using (var rec = new ServerSockets.RecycledPacket())
                            {
                                var stream = rec.GetStream();
                                if (!Owner.MySpells.ClientSpells.ContainsKey((ushort)Role.Flags.SpellID.ScrenSword))
                                    Owner.MySpells.Add(stream, (ushort)Role.Flags.SpellID.ScrenSword);
                                if (!Owner.MySpells.ClientSpells.ContainsKey((ushort)Role.Flags.SpellID.FastBlader))
                                    Owner.MySpells.Add(stream, (ushort)Role.Flags.SpellID.FastBlader);
                                if (!Owner.MySpells.ClientSpells.ContainsKey((ushort)Role.Flags.SpellID.Cyclone))
                                    Owner.MySpells.Add(stream, (ushort)Role.Flags.SpellID.Cyclone);
                            }
                        }
                        if (_level < 40 && value >= 40)
                        {
                            using (var rec = new ServerSockets.RecycledPacket())
                            {
                                var stream = rec.GetStream();
                                if (!Owner.MySpells.ClientSpells.ContainsKey((ushort)Role.Flags.SpellID.Accuracy))
                                    Owner.MySpells.Add(stream, (ushort)Role.Flags.SpellID.Accuracy);
                                if (!Owner.MySpells.ClientSpells.ContainsKey((ushort)Role.Flags.SpellID.Golem))
                                    Owner.MySpells.Add(stream, (ushort)Role.Flags.SpellID.Golem);
                                if (!Owner.MySpells.ClientSpells.ContainsKey((ushort)Role.Flags.SpellID.Hercules))
                                    Owner.MySpells.Add(stream, (ushort)Role.Flags.SpellID.Hercules);
                                if (!Owner.MySpells.ClientSpells.ContainsKey((ushort)Role.Flags.SpellID.SpiritHealing))
                                    Owner.MySpells.Add(stream, (ushort)Role.Flags.SpellID.SpiritHealing);

                                if (!Owner.MySpells.ClientSpells.ContainsKey((ushort)Role.Flags.SpellID.ScrenSword))
                                    Owner.MySpells.Add(stream, (ushort)Role.Flags.SpellID.ScrenSword);
                                if (!Owner.MySpells.ClientSpells.ContainsKey((ushort)Role.Flags.SpellID.FastBlader))
                                    Owner.MySpells.Add(stream, (ushort)Role.Flags.SpellID.FastBlader);
                                if (!Owner.MySpells.ClientSpells.ContainsKey((ushort)Role.Flags.SpellID.MonsterHunter))
                                    Owner.MySpells.Add(stream, (ushort)Role.Flags.SpellID.MonsterHunter);

                                if (!Owner.MySpells.ClientSpells.ContainsKey((ushort)Role.Flags.SpellID.UndyingWill))
                                    Owner.MySpells.Add(stream, (ushort)Role.Flags.SpellID.UndyingWill);
                                if (Reborn == 2 && Database.AtributesStatus.IsTrojan(Owner.Player.FirstClass) && Database.AtributesStatus.IsTrojan(Owner.Player.SecoundeClass))
                                {
                                    if (!Owner.MySpells.ClientSpells.ContainsKey((ushort)Role.Flags.SpellID.DragonWhirl))
                                        Owner.MySpells.Add(stream, (ushort)Role.Flags.SpellID.DragonWhirl);
                                }
                            }
                        }
                    }
                    else if (Database.AtributesStatus.IsWarrior(Class))
                    {
                        if (_level < 40 && value >= 40)
                        {
                            using (var rec = new ServerSockets.RecycledPacket())
                            {
                                var stream = rec.GetStream();
                                if (!Owner.MySpells.ClientSpells.ContainsKey((ushort)Role.Flags.SpellID.Dash))
                                    Owner.MySpells.Add(stream, (ushort)Role.Flags.SpellID.Dash);
                                if (!Owner.MySpells.ClientSpells.ContainsKey((ushort)Role.Flags.SpellID.ShieldBlock))
                                    Owner.MySpells.Add(stream, (ushort)Role.Flags.SpellID.ShieldBlock);
                                if (!Owner.MySpells.ClientSpells.ContainsKey((ushort)Role.Flags.SpellID.FlyingMoon))
                                    Owner.MySpells.Add(stream, (ushort)Role.Flags.SpellID.FlyingMoon);
                                if (!Owner.MySpells.ClientSpells.ContainsKey((ushort)Role.Flags.SpellID.MagicDefender))
                                    Owner.MySpells.Add(stream, (ushort)Role.Flags.SpellID.MagicDefender);
                                if (!Owner.MySpells.ClientSpells.ContainsKey((ushort)Role.Flags.SpellID.WaveofBlood))
                                    Owner.MySpells.Add(stream, (ushort)Role.Flags.SpellID.WaveofBlood);
                                if (!Owner.MySpells.ClientSpells.ContainsKey((ushort)Role.Flags.SpellID.ScarofEarth))
                                    Owner.MySpells.Add(stream, (ushort)Role.Flags.SpellID.ScarofEarth);
                            }//اعمل انت الصح لحد مظبت السماعه
                        }
                        if (_level < 70 && value >= 70)
                        {
                            using (var rec = new ServerSockets.RecycledPacket())
                            {
                                var stream = rec.GetStream();
                                if (!Owner.MySpells.ClientSpells.ContainsKey((ushort)Role.Flags.SpellID.MagicDefender))
                                    Owner.MySpells.Add(stream, (ushort)Role.Flags.SpellID.MagicDefender);
                                if (!Owner.MySpells.ClientSpells.ContainsKey((ushort)Role.Flags.SpellID.DefensiveStance))
                                    Owner.MySpells.Add(stream, (ushort)Role.Flags.SpellID.DefensiveStance);
                                if (!Owner.MySpells.ClientSpells.ContainsKey((ushort)Role.Flags.SpellID.TwistofWar))
                                    Owner.MySpells.Add(stream, (ushort)Role.Flags.SpellID.TwistofWar);
                            }
                        }
                        if (_level < 100 && value >= 100)
                        {
                            using (var rec = new ServerSockets.RecycledPacket())
                            {
                                var stream = rec.GetStream();
                                if (!Owner.MySpells.ClientSpells.ContainsKey((ushort)Role.Flags.SpellID.Backfire))
                                    Owner.MySpells.Add(stream, (ushort)Role.Flags.SpellID.Backfire);

                            }
                        }
                    }
                    else if (Database.AtributesStatus.IsArcher(Class))
                    {
                        if (_level < 23 && value >= 23)
                        {
                            using (var rec = new ServerSockets.RecycledPacket())
                            {
                                var stream = rec.GetStream();
                                if (!Owner.MySpells.ClientSpells.ContainsKey((ushort)Role.Flags.SpellID.ScatterFire))
                                    Owner.MySpells.Add(stream, (ushort)Role.Flags.SpellID.ScatterFire);
                            }
                        }
                        if (_level < 40 && value >= 40)
                        {
                            using (var rec = new ServerSockets.RecycledPacket())
                            {
                                var stream = rec.GetStream();
                                if (!Owner.MySpells.ClientSpells.ContainsKey((ushort)Role.Flags.SpellID.PathOfShadow))
                                    Owner.MySpells.Add(stream, (ushort)Role.Flags.SpellID.PathOfShadow);
                                if (!Owner.MySpells.ClientSpells.ContainsKey((ushort)Role.Flags.SpellID.BladeFlurry))
                                    Owner.MySpells.Add(stream, (ushort)Role.Flags.SpellID.BladeFlurry);
                                if (!Owner.MySpells.ClientSpells.ContainsKey((ushort)Role.Flags.SpellID.MortalWound))
                                    Owner.MySpells.Add(stream, (ushort)Role.Flags.SpellID.MortalWound);
                            }
                        }
                        if (_level < 46 && value >= 46)
                        {
                            using (var rec = new ServerSockets.RecycledPacket())
                            {
                                var stream = rec.GetStream();
                                if (!Owner.MySpells.ClientSpells.ContainsKey((ushort)Role.Flags.SpellID.RapidFire))
                                    Owner.MySpells.Add(stream, (ushort)Role.Flags.SpellID.RapidFire);
                            }
                        }
                        if (_level < 50 && value >= 50)
                        {
                            using (var rec = new ServerSockets.RecycledPacket())
                            {
                                var stream = rec.GetStream();
                                if (!Owner.MySpells.ClientSpells.ContainsKey((ushort)Role.Flags.SpellID.KineticSpark))
                                    Owner.MySpells.Add(stream, (ushort)Role.Flags.SpellID.KineticSpark);
                            }
                        }
                        if (_level < 70 && value >= 70)
                        {
                            using (var rec = new ServerSockets.RecycledPacket())
                            {
                                var stream = rec.GetStream();
                                if (!Owner.MySpells.ClientSpells.ContainsKey((ushort)Role.Flags.SpellID.Fly))
                                    Owner.MySpells.Add(stream, (ushort)Role.Flags.SpellID.Fly);
                                if (!Owner.MySpells.ClientSpells.ContainsKey((ushort)Role.Flags.SpellID.ArrowRain))
                                    Owner.MySpells.Add(stream, (ushort)Role.Flags.SpellID.ArrowRain);
                                if (!Owner.MySpells.ClientSpells.ContainsKey((ushort)Role.Flags.SpellID.BlisteringWave))
                                    Owner.MySpells.Add(stream, (ushort)Role.Flags.SpellID.BlisteringWave);
                            }
                        }
                        if (_level < 71 && value >= 71)
                        {
                            using (var rec = new ServerSockets.RecycledPacket())
                            {
                                var stream = rec.GetStream();
                                if (!Owner.MySpells.ClientSpells.ContainsKey((ushort)Role.Flags.SpellID.Intensify))
                                    Owner.MySpells.Add(stream, (ushort)Role.Flags.SpellID.Intensify);
                            }
                        }
                        if (_level < 90 && value >= 90)
                        {
                            using (var rec = new ServerSockets.RecycledPacket())
                            {
                                var stream = rec.GetStream();
                                if (!Owner.MySpells.ClientSpells.ContainsKey((ushort)Role.Flags.SpellID.SpiritFocus))
                                    Owner.MySpells.Add(stream, (ushort)Role.Flags.SpellID.SpiritFocus);
                            }
                        }
                        if (_level < 100 && value >= 100)
                        {
                            using (var rec = new ServerSockets.RecycledPacket())
                            {
                                var stream = rec.GetStream();
                                if (!Owner.MySpells.ClientSpells.ContainsKey((ushort)Role.Flags.SpellID.DaggerStorm))
                                    Owner.MySpells.Add(stream, (ushort)Role.Flags.SpellID.DaggerStorm);
                            }
                        }
                    }
                    else if (Database.AtributesStatus.IsNinja(Class))
                    {
                        if (_level < 20 && value >= 20)
                        {
                            using (var rec = new ServerSockets.RecycledPacket())
                            {
                                var stream = rec.GetStream();
                                if (!Owner.MySpells.ClientSpells.ContainsKey((ushort)Role.Flags.SpellID.MortalDrag))
                                    Owner.MySpells.Add(stream, (ushort)Role.Flags.SpellID.MortalDrag);
                                if (!Owner.MySpells.ClientSpells.ContainsKey((ushort)Role.Flags.SpellID.ToxicFog))
                                    Owner.MySpells.Add(stream, (ushort)Role.Flags.SpellID.ToxicFog);
                            }
                        }
                        if (_level < 40 && value >= 40)
                        {
                            using (var rec = new ServerSockets.RecycledPacket())
                            {
                                var stream = rec.GetStream();
                                if (!Owner.MySpells.ClientSpells.ContainsKey((ushort)Role.Flags.SpellID.TwofoldBlades))
                                    Owner.MySpells.Add(stream, (ushort)Role.Flags.SpellID.TwofoldBlades);
                                if (!Owner.MySpells.ClientSpells.ContainsKey((ushort)Role.Flags.SpellID.BloodyScythe))
                                    Owner.MySpells.Add(stream, (ushort)Role.Flags.SpellID.BloodyScythe);
                            }
                        }
                        if (_level < 70 && value >= 70)
                        {
                            using (var rec = new ServerSockets.RecycledPacket())
                            {
                                var stream = rec.GetStream();
                                if (!Owner.MySpells.ClientSpells.ContainsKey((ushort)Role.Flags.SpellID.ShurikenVortex))
                                    Owner.MySpells.Add(stream, (ushort)Role.Flags.SpellID.ShurikenVortex);
                            }
                        }
                        if (_level < 100 && value >= 100)
                        {
                            using (var rec = new ServerSockets.RecycledPacket())
                            {
                                var stream = rec.GetStream();
                                if (!Owner.MySpells.ClientSpells.ContainsKey((ushort)Role.Flags.SpellID.ArcherBane))
                                    Owner.MySpells.Add(stream, (ushort)Role.Flags.SpellID.ArcherBane);
                            }
                        }
                    }
                    else if (Database.AtributesStatus.IsMonk(Class))
                    {
                        if (_level < 20 && value >= 20)
                        {
                            using (var rec = new ServerSockets.RecycledPacket())
                            {
                                var stream = rec.GetStream();
                                if (!Owner.MySpells.ClientSpells.ContainsKey((ushort)Role.Flags.SpellID.TyrantAura))
                                    Owner.MySpells.Add(stream, (ushort)Role.Flags.SpellID.TyrantAura);
                                if (!Owner.MySpells.ClientSpells.ContainsKey((ushort)Role.Flags.SpellID.FendAura))
                                    Owner.MySpells.Add(stream, (ushort)Role.Flags.SpellID.FendAura);
                            }
                        }
                        if (_level < 40 && value >= 40)
                        {
                            using (var rec = new ServerSockets.RecycledPacket())
                            {
                                var stream = rec.GetStream();
                                if (!Owner.MySpells.ClientSpells.ContainsKey((ushort)Role.Flags.SpellID.RadiantPalm))
                                    Owner.MySpells.Add(stream, (ushort)Role.Flags.SpellID.RadiantPalm);
                                if (!Owner.MySpells.ClientSpells.ContainsKey((ushort)Role.Flags.SpellID.Serenity))
                                    Owner.MySpells.Add(stream, (ushort)Role.Flags.SpellID.Serenity);
                            }
                        }
                        if (_level < 70 && value >= 70)
                        {
                            using (var rec = new ServerSockets.RecycledPacket())
                            {
                                var stream = rec.GetStream();
                                if (!Owner.MySpells.ClientSpells.ContainsKey((ushort)Role.Flags.SpellID.Tranquility))
                                    Owner.MySpells.Add(stream, (ushort)Role.Flags.SpellID.Tranquility);
                            }
                        }
                        if (_level < 100 && value >= 100)
                        {
                            using (var rec = new ServerSockets.RecycledPacket())
                            {
                                var stream = rec.GetStream();
                                if (!Owner.MySpells.ClientSpells.ContainsKey((ushort)Role.Flags.SpellID.Compassion))
                                    Owner.MySpells.Add(stream, (ushort)Role.Flags.SpellID.Compassion);
                                if (!Owner.MySpells.ClientSpells.ContainsKey((ushort)Role.Flags.SpellID.EarthAura))
                                    Owner.MySpells.Add(stream, (ushort)Role.Flags.SpellID.EarthAura);
                                if (!Owner.MySpells.ClientSpells.ContainsKey((ushort)Role.Flags.SpellID.FireAura))
                                    Owner.MySpells.Add(stream, (ushort)Role.Flags.SpellID.FireAura);
                                if (!Owner.MySpells.ClientSpells.ContainsKey((ushort)Role.Flags.SpellID.MetalAura))
                                    Owner.MySpells.Add(stream, (ushort)Role.Flags.SpellID.MetalAura);
                                if (!Owner.MySpells.ClientSpells.ContainsKey((ushort)Role.Flags.SpellID.WatherAura))
                                    Owner.MySpells.Add(stream, (ushort)Role.Flags.SpellID.WatherAura);
                                if (!Owner.MySpells.ClientSpells.ContainsKey((ushort)Role.Flags.SpellID.WoodAura))
                                    Owner.MySpells.Add(stream, (ushort)Role.Flags.SpellID.WoodAura);
                            }
                        }
                    }
                    else if (Database.AtributesStatus.IsPirate(Class))
                    {
                        if (_level < 15 && value >= 15)
                        {
                            using (var rec = new ServerSockets.RecycledPacket())
                            {
                                var stream = rec.GetStream();
                                if (!Owner.MySpells.ClientSpells.ContainsKey((ushort)Role.Flags.SpellID.Golem))
                                    Owner.MySpells.Add(stream, (ushort)Role.Flags.SpellID.Golem);
                                if (!Owner.MySpells.ClientSpells.ContainsKey((ushort)Role.Flags.SpellID.Windstorm))
                                    Owner.MySpells.Add(stream, (ushort)Role.Flags.SpellID.Windstorm);
                                if (!Owner.MySpells.ClientSpells.ContainsKey((ushort)Role.Flags.SpellID.Blackspot))
                                    Owner.MySpells.Add(stream, (ushort)Role.Flags.SpellID.Blackspot);
                            }
                        }
                        if (_level < 20 && value >= 20)
                        {
                            using (var rec = new ServerSockets.RecycledPacket())
                            {
                                var stream = rec.GetStream();
                                if (!Owner.MySpells.ClientSpells.ContainsKey((ushort)Role.Flags.SpellID.GaleBomb))
                                    Owner.MySpells.Add(stream, (ushort)Role.Flags.SpellID.GaleBomb);
                            }
                        }
                        if (_level < 40 && value >= 40)
                        {
                            using (var rec = new ServerSockets.RecycledPacket())
                            {
                                var stream = rec.GetStream();
                                if (!Owner.MySpells.ClientSpells.ContainsKey((ushort)Role.Flags.SpellID.AdrenalineRush))
                                    Owner.MySpells.Add(stream, (ushort)Role.Flags.SpellID.AdrenalineRush);
                                if (!Owner.MySpells.ClientSpells.ContainsKey((ushort)Role.Flags.SpellID.EagleEye))
                                    Owner.MySpells.Add(stream, (ushort)Role.Flags.SpellID.EagleEye);
                                if (!Owner.MySpells.ClientSpells.ContainsKey((ushort)Role.Flags.SpellID.BlackbeardsRage))
                                    Owner.MySpells.Add(stream, (ushort)Role.Flags.SpellID.BlackbeardsRage);
                            }
                        }
                        if (_level < 70 && value >= 70)
                        {
                            using (var rec = new ServerSockets.RecycledPacket())
                            {
                                var stream = rec.GetStream();
                                if (!Owner.MySpells.ClientSpells.ContainsKey((ushort)Role.Flags.SpellID.KrakensRevenge))
                                    Owner.MySpells.Add(stream, (ushort)Role.Flags.SpellID.KrakensRevenge);
                            }
                        }
                    }
                    else if (Database.AtributesStatus.IsWindWalker(Class))
                    {
                        if (_level < 3 && value >= 3)
                        {
                            using (var rec = new ServerSockets.RecycledPacket())
                            {
                                var stream = rec.GetStream();
                                if (!Owner.MySpells.ClientSpells.ContainsKey((ushort)Role.Flags.SpellID.Omnipotence))
                                    Owner.MySpells.Add(stream, (ushort)Role.Flags.SpellID.Omnipotence);
                                if (!Owner.MySpells.ClientSpells.ContainsKey((ushort)Role.Flags.SpellID.JusticeChant))
                                    Owner.MySpells.Add(stream, (ushort)Role.Flags.SpellID.JusticeChant);
                            }
                        }
                        if (_level < 15 && value >= 15)
                        {
                            using (var rec = new ServerSockets.RecycledPacket())
                            {
                                var stream = rec.GetStream();
                                if (!Owner.MySpells.ClientSpells.ContainsKey((ushort)Role.Flags.SpellID.SwirlingStorm))
                                    Owner.MySpells.Add(stream, (ushort)Role.Flags.SpellID.SwirlingStorm);
                                if (!Owner.MySpells.ClientSpells.ContainsKey((ushort)Role.Flags.SpellID.ShadowofChaser))
                                    Owner.MySpells.Add(stream, (ushort)Role.Flags.SpellID.ShadowofChaser);
                                if (!Owner.MySpells.ClientSpells.ContainsKey((ushort)Role.Flags.SpellID.BurntFrost))
                                    Owner.MySpells.Add(stream, (ushort)Role.Flags.SpellID.BurntFrost);
                                if (!Owner.MySpells.ClientSpells.ContainsKey((ushort)Role.Flags.SpellID.HealingSnow))
                                    Owner.MySpells.Add(stream, (ushort)Role.Flags.SpellID.HealingSnow);
                            }
                        }
                        if (_level < 40 && value >= 40)
                        {
                            using (var rec = new ServerSockets.RecycledPacket())
                            {
                                var stream = rec.GetStream();
                                if (!Owner.MySpells.ClientSpells.ContainsKey((ushort)Role.Flags.SpellID.RageofWar))
                                    Owner.MySpells.Add(stream, (ushort)Role.Flags.SpellID.RageofWar);
                                if (!Owner.MySpells.ClientSpells.ContainsKey((ushort)Role.Flags.SpellID.HorrorofStomper))
                                    Owner.MySpells.Add(stream, (ushort)Role.Flags.SpellID.HorrorofStomper);
                                if (!Owner.MySpells.ClientSpells.ContainsKey((ushort)Role.Flags.SpellID.TripleBlasts))
                                    Owner.MySpells.Add(stream, (ushort)Role.Flags.SpellID.TripleBlasts);
                            }
                        }
                        if (_level < 70 && value >= 70)
                        {
                            using (var rec = new ServerSockets.RecycledPacket())
                            {
                                var stream = rec.GetStream();
                                if (!Owner.MySpells.ClientSpells.ContainsKey((ushort)Role.Flags.SpellID.Thundercloud))
                                    Owner.MySpells.Add(stream, (ushort)Role.Flags.SpellID.Thundercloud);
                                if (!Owner.MySpells.ClientSpells.ContainsKey((ushort)Role.Flags.SpellID.PeaceofStomper))
                                    Owner.MySpells.Add(stream, (ushort)Role.Flags.SpellID.PeaceofStomper);
                                if (!Owner.MySpells.ClientSpells.ContainsKey((ushort)Role.Flags.SpellID.ChillingSnow))
                                    Owner.MySpells.Add(stream, (ushort)Role.Flags.SpellID.ChillingSnow);

                            }
                        }
                        if (_level < 100 && value >= 100)
                        {
                            using (var rec = new ServerSockets.RecycledPacket())
                            {
                                var stream = rec.GetStream();
                                if (!Owner.MySpells.ClientSpells.ContainsKey((ushort)Role.Flags.SpellID.RevengeTail))
                                    Owner.MySpells.Add(stream, (ushort)Role.Flags.SpellID.RevengeTail);
                                if (!Owner.MySpells.ClientSpells.ContainsKey((ushort)Role.Flags.SpellID.FreezingPelter))
                                    Owner.MySpells.Add(stream, (ushort)Role.Flags.SpellID.FreezingPelter);
                                if (!Owner.MySpells.ClientSpells.ContainsKey((ushort)Role.Flags.SpellID.Thunderbolt))
                                    Owner.MySpells.Add(stream, (ushort)Role.Flags.SpellID.Thunderbolt);
                            }
                        }

                    }
                    else if (Database.AtributesStatus.IsLee(Class))
                    {
                        if (_level < 40 && value >= 40)
                        {
                            using (var rec = new ServerSockets.RecycledPacket())
                            {
                                var stream = rec.GetStream();
                                if (!Owner.MySpells.ClientSpells.ContainsKey((ushort)Role.Flags.SpellID.SpeedKick))
                                    Owner.MySpells.Add(stream, (ushort)Role.Flags.SpellID.SpeedKick);
                                if (!Owner.MySpells.ClientSpells.ContainsKey((ushort)Role.Flags.SpellID.ViolentKick))
                                    Owner.MySpells.Add(stream, (ushort)Role.Flags.SpellID.ViolentKick);
                                if (!Owner.MySpells.ClientSpells.ContainsKey((ushort)Role.Flags.SpellID.StormKick))
                                    Owner.MySpells.Add(stream, (ushort)Role.Flags.SpellID.StormKick);
                            }
                        }
                        if (_level < 70 && value >= 70)
                        {
                            using (var rec = new ServerSockets.RecycledPacket())
                            {
                                var stream = rec.GetStream();
                                if (!Owner.MySpells.ClientSpells.ContainsKey((ushort)Role.Flags.SpellID.CrackingSwipe))
                                    Owner.MySpells.Add(stream, (ushort)Role.Flags.SpellID.CrackingSwipe);
                                if (!Owner.MySpells.ClientSpells.ContainsKey((ushort)Role.Flags.SpellID.DragonRoar))
                                    Owner.MySpells.Add(stream, (ushort)Role.Flags.SpellID.DragonRoar);
                                if (!Owner.MySpells.ClientSpells.ContainsKey((ushort)Role.Flags.SpellID.DragonSwing))
                                    Owner.MySpells.Add(stream, (ushort)Role.Flags.SpellID.DragonSwing);
                                if (Reborn == 2 && Database.AtributesStatus.IsLee(Owner.Player.FirstClass) && Database.AtributesStatus.IsLee(Owner.Player.SecoundeClass))
                                {
                                    if (!Owner.MySpells.ClientSpells.ContainsKey((ushort)Role.Flags.SpellID.DragonFury))
                                        Owner.MySpells.Add(stream, (ushort)Role.Flags.SpellID.DragonFury);
                                }
                            }
                        }
                        if (_level < 100 && value >= 100)
                        {
                            using (var rec = new ServerSockets.RecycledPacket())
                            {
                                var stream = rec.GetStream();
                                if (!Owner.MySpells.ClientSpells.ContainsKey((ushort)Role.Flags.SpellID.DragonSlash))
                                    Owner.MySpells.Add(stream, (ushort)Role.Flags.SpellID.DragonSlash);
                                if (!Owner.MySpells.ClientSpells.ContainsKey((ushort)Role.Flags.SpellID.SplittingSwipe))
                                    Owner.MySpells.Add(stream, (ushort)Role.Flags.SpellID.SplittingSwipe);
                            }
                        }
                    }
                    else if (Database.AtributesStatus.IsWater(Class))
                    {
                        if (_level < 40 && value >= 40)
                        {
                            using (var rec = new ServerSockets.RecycledPacket())
                            {
                                var stream = rec.GetStream();
                                if (!Owner.MySpells.ClientSpells.ContainsKey((ushort)Role.Flags.SpellID.HealingRain))
                                    Owner.MySpells.Add(stream, (ushort)Role.Flags.SpellID.HealingRain);
                                if (!Owner.MySpells.ClientSpells.ContainsKey((ushort)Role.Flags.SpellID.StarofAccuracy))
                                    Owner.MySpells.Add(stream, (ushort)Role.Flags.SpellID.StarofAccuracy);
                                if (!Owner.MySpells.ClientSpells.ContainsKey((ushort)Role.Flags.SpellID.Revive))
                                    Owner.MySpells.Add(stream, (ushort)Role.Flags.SpellID.Revive);
                                if (!Owner.MySpells.ClientSpells.ContainsKey((ushort)Role.Flags.SpellID.SpeedLightning))
                                    Owner.MySpells.Add(stream, (ushort)Role.Flags.SpellID.SpeedLightning);
                                if (!Owner.MySpells.ClientSpells.ContainsKey((ushort)Role.Flags.SpellID.Vulcano))
                                    Owner.MySpells.Add(stream, (ushort)Role.Flags.SpellID.Vulcano);

                                if (Reborn == 2 && Database.AtributesStatus.IsWater(Owner.Player.FirstClass) && Database.AtributesStatus.IsWater(Owner.Player.SecoundeClass))
                                {
                                    if (!Owner.MySpells.ClientSpells.ContainsKey((ushort)Role.Flags.SpellID.AzureShield))
                                        Owner.MySpells.Add(stream, (ushort)Role.Flags.SpellID.AzureShield);
                                }

                            }
                        }
                        if (_level < 44 && value >= 44)
                        {
                            using (var rec = new ServerSockets.RecycledPacket())
                            {
                                var stream = rec.GetStream();
                                if (!Owner.MySpells.ClientSpells.ContainsKey((ushort)Role.Flags.SpellID.Meditation))
                                    Owner.MySpells.Add(stream, (ushort)Role.Flags.SpellID.Meditation);
                            }
                        }
                        if (_level < 50 && value >= 50)
                        {
                            using (var rec = new ServerSockets.RecycledPacket())
                            {
                                var stream = rec.GetStream();
                                if (!Owner.MySpells.ClientSpells.ContainsKey((ushort)Role.Flags.SpellID.MagicShield))
                                    Owner.MySpells.Add(stream, (ushort)Role.Flags.SpellID.MagicShield);
                            }
                        }
                        if (_level < 55 && value >= 55)
                        {
                            using (var rec = new ServerSockets.RecycledPacket())
                            {
                                var stream = rec.GetStream();
                                if (!Owner.MySpells.ClientSpells.ContainsKey((ushort)Role.Flags.SpellID.Stigma))
                                    Owner.MySpells.Add(stream, (ushort)Role.Flags.SpellID.Stigma);
                            }
                        }
                        if (_level < 60 && value >= 60)
                        {
                            using (var rec = new ServerSockets.RecycledPacket())
                            {
                                var stream = rec.GetStream();
                                if (!Owner.MySpells.ClientSpells.ContainsKey((ushort)Role.Flags.SpellID.Invisibility))
                                    Owner.MySpells.Add(stream, (ushort)Role.Flags.SpellID.Invisibility);
                            }
                        }
                        if (_level < 70 && value >= 70)
                        {
                            using (var rec = new ServerSockets.RecycledPacket())
                            {
                                var stream = rec.GetStream();
                                if (Database.AtributesStatus.IsWater(Class))
                                {
                                    if (!Owner.MySpells.ClientSpells.ContainsKey((ushort)Role.Flags.SpellID.Pray))
                                        Owner.MySpells.Add(stream, (ushort)Role.Flags.SpellID.Pray);
                                }
                            }
                        }
                        if (_level < 81 && value >= 81)
                        {
                            using (var rec = new ServerSockets.RecycledPacket())
                            {
                                var stream = rec.GetStream();
                                if (!Owner.MySpells.ClientSpells.ContainsKey((ushort)Role.Flags.SpellID.AdvancedCure))
                                    Owner.MySpells.Add(stream, (ushort)Role.Flags.SpellID.AdvancedCure);
                            }
                        }
                        if (_level < 94 && value >= 94)
                        {
                            using (var rec = new ServerSockets.RecycledPacket())
                            {
                                var stream = rec.GetStream();
                                //    if (!Owner.MySpells.ClientSpells.ContainsKey((ushort)Role.Flags.SpellID.Nectar))
                                //        Owner.MySpells.Add(stream, (ushort)Role.Flags.SpellID.Nectar);
                            }
                        }
                    }
                    else if (Database.AtributesStatus.IsFire(Class))
                    {
                        if (_level < 40 && value >= 40)
                        {
                            using (var rec = new ServerSockets.RecycledPacket())
                            {
                                var stream = rec.GetStream();
                                if (!Owner.MySpells.ClientSpells.ContainsKey((ushort)Role.Flags.SpellID.SpeedLightning))
                                    Owner.MySpells.Add(stream, (ushort)Role.Flags.SpellID.SpeedLightning);
                                if (!Owner.MySpells.ClientSpells.ContainsKey((ushort)Role.Flags.SpellID.Vulcano))
                                    Owner.MySpells.Add(stream, (ushort)Role.Flags.SpellID.Vulcano);

                                if (Reborn == 2 && Database.AtributesStatus.IsFire(Owner.Player.FirstClass) && Database.AtributesStatus.IsFire(Owner.Player.SecoundeClass))
                                {
                                    if (!Owner.MySpells.ClientSpells.ContainsKey((ushort)Role.Flags.SpellID.HeavenBlade))
                                        Owner.MySpells.Add(stream, (ushort)Role.Flags.SpellID.HeavenBlade);
                                }

                            }
                        }
                        if (_level < 44 && value >= 44)
                        {
                            using (var rec = new ServerSockets.RecycledPacket())
                            {
                                var stream = rec.GetStream();
                                if (!Owner.MySpells.ClientSpells.ContainsKey((ushort)Role.Flags.SpellID.Meditation))
                                    Owner.MySpells.Add(stream, (ushort)Role.Flags.SpellID.Meditation);
                            }
                        }
                        if (_level < 52 && value >= 52)
                        {
                            using (var rec = new ServerSockets.RecycledPacket())
                            {
                                var stream = rec.GetStream();
                                if (!Owner.MySpells.ClientSpells.ContainsKey((ushort)Role.Flags.SpellID.FireMeteor))
                                    Owner.MySpells.Add(stream, (ushort)Role.Flags.SpellID.FireMeteor);
                            }
                        }
                        if (_level < 55 && value >= 55)
                        {
                            using (var rec = new ServerSockets.RecycledPacket())
                            {
                                var stream = rec.GetStream();
                                if (!Owner.MySpells.ClientSpells.ContainsKey((ushort)Role.Flags.SpellID.FireRing))
                                    Owner.MySpells.Add(stream, (ushort)Role.Flags.SpellID.FireRing);
                            }
                        }
                        if (_level < 65 && value >= 65)
                        {
                            using (var rec = new ServerSockets.RecycledPacket())
                            {
                                var stream = rec.GetStream();
                                if (!Owner.MySpells.ClientSpells.ContainsKey((ushort)Role.Flags.SpellID.FireCircle))
                                    Owner.MySpells.Add(stream, (ushort)Role.Flags.SpellID.FireCircle);
                            }
                        }
                        if (_level < 82 && value >= 82)
                        {
                            using (var rec = new ServerSockets.RecycledPacket())
                            {
                                var stream = rec.GetStream();
                                if (!Owner.MySpells.ClientSpells.ContainsKey((ushort)Role.Flags.SpellID.Bomb))
                                    Owner.MySpells.Add(stream, (ushort)Role.Flags.SpellID.Bomb);
                            }
                        }
                    }

                    else if (Database.AtributesStatus.IsThunderStriker(Class))
                    {
                        using (var rec = new ServerSockets.RecycledPacket())
                        {
                            var stream = rec.GetStream();
                            if (!Owner.MySpells.ClientSpells.ContainsKey((ushort)Role.Flags.SpellID.WindstormBattleaxe))
                                Owner.MySpells.Add(stream, (ushort)Role.Flags.SpellID.WindstormBattleaxe);
                            if (!Owner.MySpells.ClientSpells.ContainsKey((ushort)Role.Flags.SpellID.DevouringStrike))
                                Owner.MySpells.Add(stream, (ushort)Role.Flags.SpellID.DevouringStrike);
                            if (!Owner.MySpells.ClientSpells.ContainsKey((ushort)Role.Flags.SpellID.SkyFall))
                                Owner.MySpells.Add(stream, (ushort)Role.Flags.SpellID.SkyFall);


                            if (!Owner.MySpells.ClientSpells.ContainsKey((ushort)Role.Flags.SpellID.NormalAttack1))
                                Owner.MySpells.Add(stream, (ushort)Role.Flags.SpellID.NormalAttack1);
                            if (!Owner.MySpells.ClientSpells.ContainsKey((ushort)Role.Flags.SpellID.NormalAttack2))
                                Owner.MySpells.Add(stream, (ushort)Role.Flags.SpellID.NormalAttack2);
                            if (!Owner.MySpells.ClientSpells.ContainsKey((ushort)Role.Flags.SpellID.NormalAttack3))
                                Owner.MySpells.Add(stream, (ushort)Role.Flags.SpellID.NormalAttack3);
                        }
                        if (_level < 3 && value >= 3)
                        {
                            using (var rec = new ServerSockets.RecycledPacket())
                            {
                                var stream = rec.GetStream();
                                if (!Owner.MySpells.ClientSpells.ContainsKey((ushort)Role.Flags.SpellID.ThunderRampage))
                                    Owner.MySpells.Add(stream, (ushort)Role.Flags.SpellID.ThunderRampage);
                            }
                        }
                        if (_level < 15 && value >= 15)
                        {
                            using (var rec = new ServerSockets.RecycledPacket())
                            {
                                var stream = rec.GetStream();
                                if (!Owner.MySpells.ClientSpells.ContainsKey((ushort)Role.Flags.SpellID.CrackingShock))
                                    Owner.MySpells.Add(stream, (ushort)Role.Flags.SpellID.CrackingShock);
                            }
                        }
                        if (_level < 40 && value >= 40)
                        {
                            using (var rec = new ServerSockets.RecycledPacket())
                            {
                                var stream = rec.GetStream();
                                if (!Owner.MySpells.ClientSpells.ContainsKey((ushort)Role.Flags.SpellID.UndyingImprinting))
                                    Owner.MySpells.Add(stream, (ushort)Role.Flags.SpellID.UndyingImprinting);
                            }
                        }
                        if (_level < 70 && value >= 70)
                        {
                            using (var rec = new ServerSockets.RecycledPacket())
                            {
                                var stream = rec.GetStream();
                                if (!Owner.MySpells.ClientSpells.ContainsKey((ushort)Role.Flags.SpellID.LightningShield))
                                    Owner.MySpells.Add(stream, (ushort)Role.Flags.SpellID.LightningShield);
                            }
                        }
                        if (_level < 100 && value >= 100)
                        {
                            using (var rec = new ServerSockets.RecycledPacket())
                            {
                                var stream = rec.GetStream();
                                if (!Owner.MySpells.ClientSpells.ContainsKey((ushort)Role.Flags.SpellID.ThunderBlast))
                                    Owner.MySpells.Add(stream, (ushort)Role.Flags.SpellID.ThunderBlast);
                                if (!Owner.MySpells.ClientSpells.ContainsKey((ushort)Role.Flags.SpellID.Megabolt))
                                    Owner.MySpells.Add(stream, (ushort)Role.Flags.SpellID.Megabolt);

                                if (Reborn == 2 && Database.AtributesStatus.IsThunderStriker(Owner.Player.FirstClass) && Database.AtributesStatus.IsThunderStriker(Owner.Player.SecoundeClass))
                                {
                                    if (!Owner.MySpells.ClientSpells.ContainsKey((ushort)Role.Flags.SpellID.HeavensWrath))
                                        Owner.MySpells.Add(stream, (ushort)Role.Flags.SpellID.HeavensWrath);
                                }
                                else if (Owner.MySpells.ClientSpells.ContainsKey((ushort)Role.Flags.SpellID.HeavensWrath))
                                    Owner.MySpells.Remove((ushort)Role.Flags.SpellID.HeavensWrath, stream);

                            }
                        }
                    }
                }
                _level = value;
                if (_level >= 140)
                {
                    _level = 140;
                    Experience = 0;
                }
            }
        }
        public long RelodMoney;
        public bool JumpBot;
        public bool Allin;
        public bool winnerall;
        public bool Massage;
        public bool Event;
        public byte Eventcode = 0;
        public bool EndTele;
        public bool EndTeleGuildWar;
        public bool EndTeleSuperGuildWar;
        public bool EndTeleCaptureTheFlag;


        public bool EndTeleClanTwin;
        public bool EndTeleClanPhoenix;
        public bool EndTeleClanApe;
        public bool EndTeleClanBird;
        public bool EndTeleClanDesert;

        public bool End2Tele;
        private readonly object _moneyLock = new object();
        private readonly object _cpsLock = new object();
        ulong _Money;

        public ulong Money
        {
            get
            {
                return _Money;
            }

            set
            {
                lock (_moneyLock)
                {
                ulong oldValue = _Money;
                ulong newValue = value;

                // حماية من Money overflow / bug
                if (newValue > (ulong.MaxValue - 1000000000000000))
                {
                    newValue = 0;

                    Owner.Socket.Disconnect();

                    Database.SystemBannedAccount.AddBan(
                        Owner.Player.UID,
                        Owner.Player.Name,
                        999999,
                        "Your Account Has Been Banned Due To a Money Loophole."
                    );

                    MyConsole.WriteLine(
                        "Player [ " + Owner.Player.Name +
                        " ] Banned To Bug [Money] .",
                        ConsoleColor.White
                    );

                    string logs =
                        "[Bug Money] [ " + Name +
                        " ] Get [ " + value +
                        " ] He Have [ " + oldValue + " ]";

                    logs += Environment.StackTrace;

                    Database.ServerDatabase.LoginQueue.Enqueue(logs);
                }

                // تحويل الجزء الزائد إلى الـ Warehouse
                if (newValue > 500000000000UL)
                {
                    ulong dif = newValue - 500000000000UL;

                    WHMoney += dif;
                    newValue -= dif;

                    this.Owner.SendSysMesage(
                        dif +
                        " Gold has transferred to your warehouse. " +
                        "You can't hold more than 500,000,000,000 into your inventory.",
                        MsgMessage.ChatMode.Monster,
                        MsgMessage.MsgColor.yellow
                    );
                }

                // تسجيل التغيير العادي
                LogGoldChange(oldValue, newValue);

                // =========================================================
                // High Gold Log
                // إجمالي Money + Warehouse لازم يكون 10 مليار أو أكثر
                // =========================================================

                const ulong HighGoldLimit = 10000000000UL;

                bool isHighGold = false;

                // حماية من ulong overflow
                if (WHMoney >= HighGoldLimit)
                {
                    isHighGold = true;
                }
                else
                {
                    ulong remaining =
                        HighGoldLimit - WHMoney;

                    if (newValue >= remaining)
                    {
                        isHighGold = true;
                    }
                }

                if (isHighGold)
                {
                    LogHighGold(
                        oldValue,
                        newValue
                    );
                }

                // تغيير القيمة بعد التسجيل
                _Money = newValue;

                using (var rec = new ServerSockets.RecycledPacket())
                {
                    var stream = rec.GetStream();

                    SendUpdate(
                        stream,
                        newValue,
                        MsgUpdate.DataType.Money
                    );
                }
                }
            }
        }


        // =============================================================
        // High Gold Logger
        // =============================================================

        private void LogHighGold(ulong oldValue, ulong newValue)
        {
            long diff;

            // حساب الفرق بدون ulong overflow
            if (newValue >= oldValue)
            {
                ulong amount = newValue - oldValue;

                if (amount > long.MaxValue)
                    return;

                diff = (long)amount;
            }
            else
            {
                ulong amount = oldValue - newValue;

                if (amount > long.MaxValue)
                    return;

                diff = -(long)amount;
            }


            // =============================================================
            // حساب إجمالي Gold = Money + Warehouse
            // مع حماية من ulong overflow
            // =============================================================

            ulong totalGold;

            if (ulong.MaxValue - newValue < WHMoney)
            {
                totalGold = ulong.MaxValue;
            }
            else
            {
                totalGold = newValue + WHMoney;
            }


            // =============================================================
            // معرفة مصدر التغيير
            // =============================================================

            string source = "Unknown";
            string file = "Unknown";
            string line = "0";

            try
            {
                System.Diagnostics.StackTrace trace =
                    new System.Diagnostics.StackTrace(true);

                for (int i = 1; i < trace.FrameCount; i++)
                {
                    System.Diagnostics.StackFrame frame =
                        trace.GetFrame(i);

                    if (frame == null)
                        continue;

                    System.Reflection.MethodBase method =
                        frame.GetMethod();

                    if (method == null)
                        continue;

                    if (method.DeclaringType == null)
                        continue;

                    string className =
                        method.DeclaringType.FullName;

                    if (string.IsNullOrEmpty(className))
                        continue;

                    // نتخطى Player
                    if (className.Contains("Player"))
                        continue;

                    // نتخطى System
                    if (className.StartsWith("System."))
                        continue;

                    source =
                        className + "." + method.Name;

                    string fileName =
                        frame.GetFileName();

                    if (!string.IsNullOrEmpty(fileName))
                    {
                        file =
                            System.IO.Path.GetFileName(fileName);
                    }

                    int lineNumber =
                        frame.GetFileLineNumber();

                    if (lineNumber > 0)
                    {
                        line =
                            lineNumber.ToString();
                    }

                    break;
                }
            }
            catch
            {
                source = "TraceError";
            }


            // =============================================================
            // نوع العملية
            // =============================================================

            string type;

            if (diff > 0)
                type = "Gold+";
            else if (diff < 0)
                type = "Gold-";
            else
                type = "Gold";


            // =============================================================
            // Log
            // =============================================================

            string log =
                "[High Money] " +
                DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") +
                " | UID=" + UID +
                " | Name=" + Name +
                " | Old=" + oldValue +
                " | New=" + newValue +
                " | Diff=" + (diff >= 0 ? "+" : "") + diff +
                " | Warehouse=" + WHMoney +
                " | TotalGold=" + totalGold +
                " | Type=" + type +
                " | Method=" + source +
                " | File=" + file +
                " | Line=" + line;


            // يظهر فوراً في Console
            MyConsole.WriteLine(
                log,
                ConsoleColor.Yellow
            );


            // يتسجل في LoginQueue
            Database.ServerDatabase.LoginQueue.Enqueue(log);
        }



        private void LogGoldChange(ulong oldValue, ulong newValue)
        {
            long diff;

            if (newValue >= oldValue)
            {
                ulong amount = newValue - oldValue;

                if (amount > long.MaxValue)
                    return;

                diff = (long)amount;
            }
            else
            {
                ulong amount = oldValue - newValue;

                if (amount > long.MaxValue)
                    return;

                diff = -(long)amount;
            }

            if (diff == 0)
                return;

            string type;

            if (diff > 0)
                type = "Money+";
            else
                type = "Money-";

            string source = "Unknown";
            string file = "Unknown";
            string line = "0";

            try
            {
                System.Diagnostics.StackTrace trace =
                    new System.Diagnostics.StackTrace(true);

                for (int i = 1; i < trace.FrameCount; i++)
                {
                    System.Diagnostics.StackFrame frame =
                        trace.GetFrame(i);

                    if (frame == null)
                        continue;

                    System.Reflection.MethodBase method =
                        frame.GetMethod();

                    if (method == null)
                        continue;

                    if (method.DeclaringType == null)
                        continue;

                    string className =
                        method.DeclaringType.FullName;

                    if (className == null)
                        continue;

                    // نتخطى كلاس Player نفسه
                    if (className.Contains("Player"))
                        continue;

                    // نتخطى System
                    if (className.StartsWith("System."))
                        continue;

                    source =
                        className + "." + method.Name;

                    string fileName =
                        frame.GetFileName();

                    if (!string.IsNullOrEmpty(fileName))
                    {
                        file =
                            System.IO.Path.GetFileName(fileName);
                    }

                    int lineNumber =
                        frame.GetFileLineNumber();

                    if (lineNumber > 0)
                    {
                        line =
                            lineNumber.ToString();
                    }

                    break;
                }
            }
            catch
            {
                source = "TraceError";
            }

            string log =
                "[" + type + "] " +
                DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") +
                " | UID=" + UID +
                " | Name=" + Name +
                " | Old=" + oldValue +
                " | New=" + newValue +
                " | Diff=" + (diff >= 0 ? "+" : "") + diff +
                " | Method=" + source +
                " | File=" + file +
                " | Line=" + line;

            Database.ServerDatabase.LoginQueue.Enqueue(log);
        }

        public ushort FrameID = 0;
        public byte WinnerBest = 0;
        public uint WarDropeFull = 0;
        public byte RebornItem = 0;

        public byte questtwin = 0;
        public byte questphoinex = 0;
        public byte questape = 0;
        public byte questdeserst = 0;
        public byte questbird = 0;


        public byte questone = 0;
        public byte questtwo = 0;
        public byte questthree = 0;
        public byte questfour = 0;
        public uint FootprintOpen = 0;
        public uint HaloActionOpen = 0;
        uint _cps;

        public uint ConquerPoints
        {
            get
            {
                return _cps;
            }

            set
            {
                lock (_cpsLock)
                {
                uint oldValue = _cps;
                uint newValue = value;

                

                if (newValue > 2147000000)
                {
                    uint dif = newValue - 2147000000;

                    CpsBank += dif;
                    newValue -= dif;

                    Owner.SendSysMesage(
                        "[ " + dif + " ] CPS has transferred to your Bank Cps. " +
                        "You can't hold more than 2,147,000,000 CPS.",
                        MsgMessage.ChatMode.Monster,
                        MsgMessage.MsgColor.yellow
                    );
                }

                // نسجل قبل تغيير القيمة
                LogCpsChange(oldValue, newValue);

                // لوج خاص باللاعبين اللي معاهم 1,000,000 CPS أو أكثر
                if (newValue >= 1000000)
                {
                    LogHighCps(oldValue, newValue);
                }

                // تغيير القيمة بعد التسجيل
                _cps = newValue;

                if (Owner.FullLoading)
                {
                    using (var rec = new ServerSockets.RecycledPacket())
                    {
                        var stream = rec.GetStream();

                        Game.MsgServer.MsgUpdate packet =
                            new Game.MsgServer.MsgUpdate(stream, UID, 1);

                        stream = packet.Append(
                            stream,
                            MsgUpdate.DataType.ConquerPoints,
                            newValue
                        );

                        stream = packet.GetArray(stream);

                        Owner.Send(stream);
                    }
                }
                }
            }
        }


        private void LogHighCps(uint oldValue, uint newValue)
        {
            long diff = (long)newValue - (long)oldValue;

            string source = "Unknown";
            string file = "Unknown";
            string line = "0";

            try
            {
                System.Diagnostics.StackTrace trace =
                    new System.Diagnostics.StackTrace(true);

                for (int i = 1; i < trace.FrameCount; i++)
                {
                    System.Diagnostics.StackFrame frame =
                        trace.GetFrame(i);

                    if (frame == null)
                        continue;

                    System.Reflection.MethodBase method =
                        frame.GetMethod();

                    if (method == null || method.DeclaringType == null)
                        continue;

                    string className = method.DeclaringType.FullName;

                    if (string.IsNullOrEmpty(className))
                        continue;

                    if (className.Contains("Player"))
                        continue;

                    if (className.StartsWith("System."))
                        continue;

                    source = className + "." + method.Name;

                    string fileName = frame.GetFileName();

                    if (!string.IsNullOrEmpty(fileName))
                        file = System.IO.Path.GetFileName(fileName);

                    int lineNumber = frame.GetFileLineNumber();

                    if (lineNumber > 0)
                        line = lineNumber.ToString();

                    break;
                }
            }
            catch
            {
                source = "TraceError";
            }

            string log =
                "[High CPs] " +
                DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") +
                " | UID=" + UID +
                " | Name=" + Name +
                " | Old=" + oldValue +
                " | New=" + newValue +
                " | Diff=" + (diff >= 0 ? "+" : "") + diff +
                " | Method=" + source +
                " | File=" + file +
                " | Line=" + line;

            Database.ServerDatabase.LoginQueue.Enqueue(log);
        }



        private void LogCpsChange(uint oldValue, uint newValue)
        {
            long diff = (long)newValue - (long)oldValue;

            if (diff == 0)
                return;

            string type;

            if (diff > 0)
                type = "CPS+";
            else
                type = "CPS-";

            string source = "Unknown";
            string file = "Unknown";
            string line = "0";

            try
            {
                System.Diagnostics.StackTrace trace =
                    new System.Diagnostics.StackTrace(true);

                for (int i = 1; i < trace.FrameCount; i++)
                {
                    System.Diagnostics.StackFrame frame =
                        trace.GetFrame(i);

                    if (frame == null)
                        continue;

                    System.Reflection.MethodBase method =
                        frame.GetMethod();

                    if (method == null)
                        continue;

                    if (method.DeclaringType == null)
                        continue;

                    string className =
                        method.DeclaringType.FullName;

                    if (className == null)
                        continue;

                    // نتخطى كلاس Player نفسه
                    if (className.Contains("Player"))
                        continue;

                    // نتخطى System
                    if (className.StartsWith("System."))
                        continue;

                    source =
                        className + "." + method.Name;

                    string fileName =
                        frame.GetFileName();

                    if (!string.IsNullOrEmpty(fileName))
                    {
                        file = System.IO.Path.GetFileName(fileName);
                    }

                    int lineNumber =
                        frame.GetFileLineNumber();

                    if (lineNumber > 0)
                        line = lineNumber.ToString();

                    break;
                }
            }
            catch
            {
                source = "TraceError";
            }

            string log =
                "[" + type + "] " +
                DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") +
                " | UID=" + UID +
                " | Name=" + Name +
                " | Old=" + oldValue +
                " | New=" + newValue +
                " | Diff=" + (diff >= 0 ? "+" : "") + diff +
                " | Method=" + source +
                " | File=" + file +
                " | Line=" + line;

            Database.ServerDatabase.LoginQueue.Enqueue(log);
        }

        long _domino;
        public long DominoCoins
        {
            get { return _domino; }
            set
            {
                _domino = value;
                if (Owner.FullLoading)
                {
                    using (var rec = new ServerSockets.RecycledPacket())
                    {
                        SendUpdate(rec.GetStream(), value, MsgUpdate.DataType.DominoCoins);
                    }
                }
            }
        }
        ulong _dominoc;
        public ulong DominoCode
        {
            get { return _dominoc; }
            set
            {
                _dominoc = value;
            }
        }
        int _bountCps;
        public int BoundConquerPoints
        {
            get { return _bountCps; }
            set
            {
                _bountCps = value;
                if (Owner.FullLoading)
                {
                    using (var rec = new ServerSockets.RecycledPacket())
                    {
                        var stream = rec.GetStream();
                        Game.MsgServer.MsgUpdate packet = new Game.MsgServer.MsgUpdate(stream, UID, 1);
                        stream = packet.Append(stream, MsgUpdate.DataType.BoundConquerPoints, value);
                        stream = packet.GetArray(stream);
                        Owner.Send(stream);
                    }
                }
            }
        }
        public ulong Experience;
        public uint VirtutePoints;

        public Time32 WeaponCombo = Time32.Now;
        public bool DeadState = true;
        int _minhitpoints;
        public unsafe int HitPoints
        {
            get { return _minhitpoints; }
            set
            {
                DeadState = value <= 0;
                _minhitpoints = value;
                if (Owner.Team != null)
                {
                    var TeamMember = Owner.Team.GetMember(UID);
                    if (TeamMember != null)
                    {
                        TeamMember.Info.MaxHitpoints = (ushort)Owner.Status.MaxHitpoints;
                        TeamMember.Info.MinMHitpoints = (ushort)value;
                        Owner.Team.SendTeamInfo(TeamMember);
                    }
                }
                if (Owner.FullLoading)
                {
                    SendUpdateHP();
                }
            }
        }

        ushort _mana;
        public unsafe ushort Mana
        {
            get { return _mana; }
            set
            {
                _mana = value;
                if (Owner.FullLoading)
                {
                    using (var rec = new ServerSockets.RecycledPacket())
                    {
                        var stream = rec.GetStream();
                        SendUpdate(stream, value, Game.MsgServer.MsgUpdate.DataType.Mana);
                    }
                }
            }
        }
        ushort _pkpoints;
        public ushort PKPoints
        {
            get { return _pkpoints; }
            set
            {
                _pkpoints = value;
                if (PKPoints > 99)
                {
                    RemoveFlag(Game.MsgServer.MsgUpdate.Flags.RedName);
                    AddFlag(Game.MsgServer.MsgUpdate.Flags.BlackName, StatusFlagsBigVector32.PermanentFlag, false, 6 * 60);
                }
                else if (PKPoints > 29)
                {
                    AddFlag(Game.MsgServer.MsgUpdate.Flags.RedName, StatusFlagsBigVector32.PermanentFlag, false, 6 * 60);
                    RemoveFlag(Game.MsgServer.MsgUpdate.Flags.BlackName);
                }
                else if (PKPoints < 30)
                {
                    RemoveFlag(Game.MsgServer.MsgUpdate.Flags.RedName);
                    RemoveFlag(Game.MsgServer.MsgUpdate.Flags.BlackName);
                }
                using (var rec = new ServerSockets.RecycledPacket())
                {
                    var stream = rec.GetStream();
                    SendUpdate(stream, PKPoints, Game.MsgServer.MsgUpdate.DataType.PKPoints);
                }
            }
        }
        public unsafe uint QuizPoints;
        public unsafe ushort Enilghten;

        DateTime _ExpireVip;
        public DateTime ExpireVip
        {
            get
            {
                return _ExpireVip;
            }
            set
            {
                _ExpireVip = value;
            }
        }
        public bool ExpireVipback;
        byte _viplevel;
        public byte VipLevel
        {
            get
            {
                return _viplevel;
            }
            set
            {

                _viplevel = value;
            }
        }
        byte _viplevelback;

        public byte VipLevelback
        {
            get
            {
                return _viplevelback;
            }
            set
            {

                _viplevelback = value;
            }
        }
        public ushort EnlightenReceive;
        ushort face;
        public unsafe ushort Face
        {
            get { return face; }
            set
            {
                face = value;
                if (Owner.FullLoading)
                {
                    using (var rec = new ServerSockets.RecycledPacket())
                    {
                        var stream = rec.GetStream();
                        SendUpdate(stream, Mesh, Game.MsgServer.MsgUpdate.DataType.Mesh);
                    }
                }
            }
        }

        public byte HairColor
        {
            get
            {
                return (byte)(Hair / 1000);
            }
            set
            {
                Hair = (ushort)((value * 1000) + (Hair % 1000));
            }
        }
        uint _hair;
        public uint Hair
        {
            get { return _hair; }
            set
            {
                _hair = value;
                if (Owner.FullLoading)
                {
                    using (var rec = new ServerSockets.RecycledPacket())
                    {
                        var stream = rec.GetStream();
                        SendUpdate(stream, _hair, Game.MsgServer.MsgUpdate.DataType.HairStyle);
                    }
                    if (Owner.HairfaceStorage != null)
                        Owner.HairfaceStorage.Add(Database.HairfaceStorageTable.Hairfaces.Where(i => i.ID == value % 1000 && i.Type == MsgHairfaceStorage.Type.Hairstyle).FirstOrDefault(), false);
                }
            }
        }

        public uint PDinamycID { get; set; }
        public uint DynamicID { get; set; }

        uint _mmmap;
        public uint Map
        {
            get { return _mmmap; }
            set { _mmmap = value; }
        }
        #region Skill

        public Extensions.Time32 PheasantBeakPassiveStamp = new Extensions.Time32();
        public Extensions.Time32 PheasantBeakStamp = new Extensions.Time32();
        public Extensions.Time32 SpellStamp = new Extensions.Time32();

        #endregion

        ushort xx, yy;
        public unsafe ushort X
        {
            get { return xx; }
            set { Px = X; xx = value; }
        }
        public unsafe ushort Y
        {
            get { return yy; }
            set { Py = Y; yy = value; }
        }

        public void ClearPreviouseCoord()
        {
            Px = 0;
            Py = 0;
        }
        public bool CanOut = false;
        public ushort Px;
        public ushort Py;
        public ushort PMapX;
        public ushort PMapY;
        public uint PMap;
        public short GetMyDistance(ushort X2, ushort Y2)
        {
            return Core.GetDistance(X, Y, X2, Y2);
        }
        public short OldGetDistance(ushort X2, ushort Y2)
        {
            return Core.GetDistance(Px, Py, X2, Y2);
        }
        public bool InView(ushort X2, ushort Y2, byte distance)
        {

            return ((OldGetDistance(X2, Y2) > distance) && GetMyDistance(X2, Y2) <= distance);
        }

        public unsafe Flags.ConquerAngle Angle = Flags.ConquerAngle.East;
        public unsafe Flags.ConquerAction Action = Flags.ConquerAction.None;
        public uint LastWalk;

        public byte ExpBallUsed = 0;
        public byte BDExp = 0;
        public DateTime JoinOnflineTG = new DateTime();
        public DateTime OnlineTrainingTime = new DateTime();
        public DateTime ReceivePointsOnlineTraining = new DateTime();
        public DateTime HeavenBlessTime = new DateTime();
        public int HeavenBlessing = 0;
        public uint OnlineTrainingPoints = 0;
        public uint HuntingBlessing = 0;
        public DateTime LastDivineGuard = new DateTime();
        public uint DExpTime = 0;
        public uint RateExp = 1;

        public uint ExpProtection = 0;
        public void CreateExpProtection(ServerSockets.Packet stream, uint Time, bool uppdate = true)
        {
            if (uppdate)
                ExpProtection = Time;
            Game.MsgServer.MsgUpdate update = new Game.MsgServer.MsgUpdate(stream, UID, 1);
            stream = update.Append(stream, Game.MsgServer.MsgUpdate.DataType.ExpProtection, new uint[5] { 0, ExpProtection, 0, 0, 0 });
            stream = update.GetArray(stream);
            Owner.Send(stream);
        }
        public unsafe void CreateExtraExpPacket(ServerSockets.Packet stream)
        {
            Game.MsgServer.MsgUpdate update = new Game.MsgServer.MsgUpdate(stream, UID, 2);
            stream = update.Append(stream, Game.MsgServer.MsgUpdate.DataType.DoubleExpTimer, new uint[7] { 0, DExpTime, 0, (uint)(RateExp * 100), 0, 0, 200 });
            stream = update.GetArray(stream);
            Owner.Send(stream);
        }
        public void AddHeavenBlessing(ServerSockets.Packet stream, int Time)
        {
            if (!ContainFlag(Game.MsgServer.MsgUpdate.Flags.HeavenBlessing))
                HeavenBlessTime = DateTime.Now;

            if (Time > 60 * 60 * 24)
                Owner.SendSysMesage("You`ve received " + Time / (60 * 60 * 24) + " days` blessing time.", Game.MsgServer.MsgMessage.ChatMode.System);
            else
            {
                Owner.SendSysMesage("You`ve received " + (Time / 60) / 60 + " hours` blessing time.", Game.MsgServer.MsgMessage.ChatMode.System);
            }
            bool None = HeavenBlessing == 0;
            HeavenBlessTime = HeavenBlessTime.AddSeconds(Time);

            HeavenBlessing += Time;
            CreateHeavenBlessPacket(stream, None);

            if (MyMentor != null)
            {
                MyMentor.Mentor_Blessing += (uint)(Time / 10000);
                Role.Instance.Associate.Member mee;
                if (MyMentor.Associat.ContainsKey(Role.Instance.Associate.Apprentice))
                {
                    if (MyMentor.Associat[Role.Instance.Associate.Apprentice].TryGetValue(UID, out mee))
                    {
                        mee.Blessing += (uint)(Time / 10000);
                    }
                }
            }

        }
        public void CreateHeavenBlessPacket(ServerSockets.Packet stream, bool ResetOnlineTraining)
        {
            if (HeavenBlessing > 0)
            {
                if (ResetOnlineTraining)
                {
                    ReceivePointsOnlineTraining = DateTime.Now.AddMinutes(1);
                    OnlineTrainingTime = DateTime.Now.AddMinutes(10);
                }
                AddFlag(Game.MsgServer.MsgUpdate.Flags.HeavenBlessing, Role.StatusFlagsBigVector32.PermanentFlag, false);
                SendUpdate(stream, HeavenBlessing, Game.MsgServer.MsgUpdate.DataType.HeavensBlessing, false);

                SendUpdate(stream, Game.MsgServer.MsgUpdate.OnlineTraining.Show, Game.MsgServer.MsgUpdate.DataType.OnlineTraining);
                if (Map == 601 || Map == 1039)
                    SendUpdate(stream, Game.MsgServer.MsgUpdate.OnlineTraining.InTraining, Game.MsgServer.MsgUpdate.DataType.OnlineTraining);
                SendString(stream, Game.MsgServer.MsgStringPacket.StringID.Effect, true, new string[1] { "bless" });
            }
        }
        public byte GetGender
        {
            get
            {
                if (Body % 10 == 8)
                    return 0;
                else
                    return 1;
            }
        }
        ushort body;
        public unsafe ushort Body
        {
            get { return body; }
            set
            {
                body = value;
                if (Owner.FullLoading)
                {
                    using (var rec = new ServerSockets.RecycledPacket())
                    {
                        var stream = rec.GetStream();
                        SendUpdate(stream, Mesh, Game.MsgServer.MsgUpdate.DataType.Mesh, true);
                    }
                }
            }
        }
        private ushort _transformationid;
        public unsafe ushort TransformationID
        {
            get
            {
                return _transformationid;
            }
            set
            {
                _transformationid = value;
                if (Owner.FullLoading)
                {
                    using (var rec = new ServerSockets.RecycledPacket())
                    {
                        var stream = rec.GetStream();
                        SendUpdate(stream, Mesh, Game.MsgServer.MsgUpdate.DataType.Mesh, true);
                    }
                }
            }
        }
        public DateTime lastUseItem;
        public bool Alive { get { return HitPoints > 0; } }


        public unsafe uint Mesh
        {
            get
            {

                return (uint)(TransformationID * 10000000 + Face * 10000 + Body);
            }

        }
        public unsafe void SendUpdate(ServerSockets.Packet stream, ulong Value, Game.MsgServer.MsgUpdate.DataType datatype, bool scren = false)
        {
            Game.MsgServer.MsgUpdate packet = new Game.MsgServer.MsgUpdate(stream, UID, 1);
            stream = packet.Append(stream, datatype, Value);
            stream = packet.GetArray(stream);
            Owner.Send(stream);
            if (scren)
            {
                View.SendView(stream, false);
            }
        }
        public unsafe void SendUpdate(ServerSockets.Packet stream, long Value, Game.MsgServer.MsgUpdate.DataType datatype, bool scren = false)
        {
            Game.MsgServer.MsgUpdate packet = new Game.MsgServer.MsgUpdate(stream, UID, 1);
            stream = packet.Append(stream, datatype, Value);
            stream = packet.GetArray(stream);
            Owner.Send(stream);
            if (scren)
            {
                View.SendView(stream, false);
            }
        }
        public unsafe void SendUpdateSlayer(ServerSockets.Packet stream, Game.MsgServer.MsgUpdate.DataType datatype, long Time, bool scren = false)
        {
            Game.MsgServer.MsgUpdate packet = new Game.MsgServer.MsgUpdate(stream, UID, 1);
            stream = packet.Append(stream, datatype, Time);
            stream = packet.GetArray(stream);
            Owner.Send(stream);
            if (scren)
                View.SendView(stream, false);
        }
        public unsafe void SendUpdate(ServerSockets.Packet stream, Game.MsgServer.MsgUpdate.Flags Flag, uint Time, uint Dmg, uint Level, Game.MsgServer.MsgUpdate.DataType datatype, bool scren = false)
        {
            Game.MsgServer.MsgUpdate packet = new Game.MsgServer.MsgUpdate(stream, UID, 1);
            stream = packet.Append(stream, datatype, (uint)Flag, Time, Dmg, Level);
            stream = packet.GetArray(stream);
            Owner.Send(stream);
            if (scren)
                View.SendView(stream, false);
        }
        public unsafe void SendUpdate(ServerSockets.Packet stream, Game.MsgServer.MsgUpdate.Flags Flag, uint Time, int Dmg, int Level, Game.MsgServer.MsgUpdate.DataType datatype, bool scren = false)
        {
            Game.MsgServer.MsgUpdate packet = new Game.MsgServer.MsgUpdate(stream, UID, 1);
            stream = packet.Append(stream, datatype, (uint)Flag, Time, Dmg, Level);
            stream = packet.GetArray(stream);
            Owner.Send(stream);
            if (scren)
                View.SendView(stream, false);
        }
        public unsafe void SendUpdate(ServerSockets.Packet stream, Game.MsgServer.MsgUpdate.Flags Flag, uint Time, int Dmg, int Level, long Dmg2, long Level2, Game.MsgServer.MsgUpdate.DataType datatype, bool scren = false)
        {
            Game.MsgServer.MsgUpdate packet = new Game.MsgServer.MsgUpdate(stream, UID, 1);
            stream = packet.Append(stream, datatype, (uint)Flag, Time, Dmg, Level, Dmg2, Level2);
            stream = packet.GetArray(stream);
            Owner.Send(stream);
            if (scren)
                View.SendView(stream, false);
        }
        public unsafe void SendUpdate(ServerSockets.Packet stream, Game.MsgServer.MsgUpdate.DataType datatype, uint Time, uint Dmg, uint Level, bool scren = false)
        {
            Game.MsgServer.MsgUpdate packet = new Game.MsgServer.MsgUpdate(stream, UID, 1);
            stream = packet.Append(stream, datatype, (uint)0, Time, Dmg, Level);
            stream = packet.GetArray(stream);

            Owner.Send(stream);
            if (scren)
                View.SendView(stream, false);
        }
        public unsafe void SendUpdate(uint[] Value, Game.MsgServer.MsgUpdate.DataType datatype, bool scren = false)
        {
            using (var rec = new ServerSockets.RecycledPacket())
            {
                var stream = rec.GetStream();

                Game.MsgServer.MsgUpdate packet = new Game.MsgServer.MsgUpdate(stream, UID, 1);
                stream = packet.Append(stream, datatype, Value);
                stream = packet.GetArray(stream);
                Owner.Send(stream);
                if (scren)
                    View.SendView(stream, false);
            }
        }
        public unsafe void UpdateVip(ServerSockets.Packet stream)
        {
            SendUpdate(stream, VipLevel, MsgUpdate.DataType.VIPLevel, false);
            if (VipLevel >= 6)
            {

                Owner.Send(stream.VipStatusCreate(MsgVipStatus.VipFlags.FullVip));
            }
            else if (VipLevel >= 1)
            {
                Owner.Send(stream.VipStatusCreate(MsgVipStatus.VipFlags.FullVip));
            }
            else
                Owner.Send(stream.VipStatusCreate(MsgVipStatus.VipFlags.None));
        }
        public unsafe void SendString(ServerSockets.Packet stream, Game.MsgServer.MsgStringPacket.StringID id, bool SendScreen, params string[] args)
        {
            Qahr.Mobile.Game.MsgServer.MsgBuilder.MsgStringProto packet1 = new Qahr.Mobile.Game.MsgServer.MsgBuilder.MsgStringProto();
            Game.MsgServer.MsgStringPacket packet = new Game.MsgServer.MsgStringPacket();
            packet1.ID = id;
            packet1.UID = UID;
            packet1.Strings = args;

            if (SendScreen)
                View.SendView(stream.StringPacketCreate(packet), true);
            else
                Owner.Send(stream.StringPacketCreate(packet));
        }
        public unsafe void SendString(ServerSockets.Packet stream, Game.MsgServer.MsgStringPacket.StringID id, uint _uid, bool SendScreen, params string[] args)
        {
            Game.MsgServer.MsgStringPacket packet = new Game.MsgServer.MsgStringPacket();
            packet.ID = id;
            packet.UID = _uid;
            packet.Strings = args;

            if (SendScreen)
                View.SendView(stream.StringPacketCreate(packet), true);
            else
                Owner.Send(stream.StringPacketCreate(packet));
        }

        public uint HeadId = 0;
        public uint GarmentId = 0;
        public uint ArmorId = 0;
        public uint LeftWeaponId = 0;
        public uint RightWeaponId = 0;
        public uint LeftWeaponAccessoryId = 0;
        public uint RightWeaponAccessoryId = 0;
        public uint SteedId = 0;
        public uint MountArmorId = 0;

        public ushort ColorArmor = 0;
        public ushort ColorShield = 0;
        public ushort ColorHelment = 0;

        public uint SteedPlus = 0;
        public uint SteedColor = 0;

        public uint HeadSoul = 0;
        public uint ArmorSoul = 0;
        public uint LeftWeapsonSoul = 0;
        public uint RightWeapsonSoul = 0;

        public uint WingId = 0;
        public byte WingPlus = 0;
        public uint WingProgress = 0;
        public uint Bottle = 0;


        public uint RealUID = 0;

        public string UnionName = "";


        public void AddMapEffect(ServerSockets.Packet stream, ushort x, ushort y, params string[] effect)
        {
            Game.MsgServer.MsgStringPacket packet = new Game.MsgServer.MsgStringPacket();
            packet.ID = MsgStringPacket.StringID.LocationEffect;
            packet.X = x;
            packet.Y = y;
            packet.Strings = effect;
            View.SendView(stream.StringPacketCreate(packet), true);


        }
        public int FairyForm = -1;

        public void ClearItemsSpawn()
        {
            HeadId = GarmentId = WingId = WingProgress = ArmorId = LeftWeaponId = RightWeaponId = LeftWeaponAccessoryId = RightWeaponAccessoryId = SteedId = MountArmorId = Bottle = 0;
            ColorArmor = ColorShield = ColorHelment = 0;
            SteedPlus = SteedColor = WingPlus = 0;
            HeadSoul = ArmorSoul = LeftWeapsonSoul = RightWeapsonSoul = 0;
        }
        public unsafe ServerSockets.Packet GetArray(ServerSockets.Packet stream, bool WindowsView)
        {
            byte prestigeRank = (byte)Database.PrestigeRanking.GetMyRank(Database.PrestigeRanking.GetIndex(Class), UID);
            stream.InitWriter();
            var proto = new SpawnPacketProto()
            {
                UID = UID,
                Mesh = Mesh,
                Head = HeadId,
                Garment = GarmentId,
                LeftWeapon = LeftWeaponId,
                LeftWeaponSoul = LeftWeapsonSoul,
                RightWeapon = RightWeaponId,
                RightWeaponSoul = RightWeapsonSoul,
                LeftWeaponAccessory = LeftWeaponAccessoryId,
                RightWeaponAccessory = RightWeaponAccessoryId,
                Steed = SteedId,
                MountArmor = MountArmorId,
                AppearanceType = (ushort)AparenceType,
                GuildRank = (uint)GuildRank,
                GuildID = GuildID,
                Wing = WingTitleHaloHide ? 0 : WingId,
                WingPlus = WingPlus,
                WingProgress = WingProgress,
                Bottle = Bottle,
                Hitpoints = (uint)HitPoints,
                X = X,
                Y = Y,
                HairStyle = Hair,
                Facing = (byte)Angle,
                Action = (ushort)Action,
                Reborn = Reborn,
                SecondRebornClass = SecoundeClass,
                FirstRebornClass = FirstClass,
                Level = Level,
                WindowSpawn = (byte)(WindowsView ? 1 : 0),
                Away = Away > 0,
                ExtraBattlePower = ExtraBattlePower,
                FlowerIcon = FairyForm,//This for FairyForm, if you're not transform in fairy default value is -1
                NobilityRank = (byte)NobilityRank,
                Armor = ArmorId,
                ArmorSoul = ArmorSoul,
                HeadSoul = HeadSoul,
                SteedColor = SteedColor,
                SteedPlus = SteedPlus,
                ClanUID = ClanUID,
                ClanRank = ClanRank,
                Title = SomeHide ? 0 : (uint)MyTitle,
                ActiveSubClasses = (byte)ActiveSublass,
                SubClass = SubClassHasPoints,
                JiangHuActive = JiangHuActive > 0,
                JiangHuTalent = JiangHuTalent,
                MaxLife = Owner.Status.MaxHitpoints,
                RealUID = RealUID,
                UnionID = InUnion ? MyUnion.UID : 0,
                UnionType = InUnion ? (uint)MyUnion.IsKingdom : 0,
                BattlePower = BattlePower,
                QuizPoints = QuizPoints,
                Class = Class,
                GuildBattlePower = GuildBattlePower,
                InvisibleArena = (byte)(Owner.IsWatching() ? 1 : 0),
                MyWing = WingTitleHaloHide ? 0 : SpecialWingID,
                MyTitle = WingTitleHaloHide ? 0 : SpecialTitleID,
                MyTitleScore = SpecialTitleScore,
                CountryCode = CountryID,
                PrestigeRank = (byte)(prestigeRank >= 1 && prestigeRank <= 3 ? prestigeRank : 0),
                InnerStrengthScore = InnerPower.TotalScore,
                MainFlag = (byte)MainFlag,
                Enlighten = Enilghten,
                Official_Harem_Guards = InUnion ? (uint)ExploitsRank : 0,
                Name = System.Text.Encoding.UTF8.GetBytes(Name ?? ""),
                FamilyName = System.Text.Encoding.UTF8.GetBytes(""),
                SyndicateName = System.Text.Encoding.UTF8.GetBytes(MyGuild != null ? MyGuild.GuildName : ""),
                LeagueName = System.Text.Encoding.UTF8.GetBytes(!InUnion ? "" : MyUnion.Name),
                MateName = System.Text.Encoding.UTF8.GetBytes(ClanName ?? ""),
                OwnerPetName = System.Text.Encoding.UTF8.GetBytes(""),
                Name134 = System.Text.Encoding.UTF8.GetBytes("")
            };
            MsgGameItem relic;
            if (Owner.Equipment.TryGetEquip(Flags.ConquerItem.Relics, out relic))
            {
                var RelicFlag = relic.RelicAttributes.Where(i => i > 0 && i.Epic);

                if (RelicFlag.Count() >= 5)
                {
                    proto.Relic = (byte)(50 + relic.RelicAttributes.Where(i => i > 0 && i.Epic).Count());
                }
                else if (RelicFlag.Count() == 1)
                    proto.Relic = (byte)(43 + relic.RelicAttributes.Where(i => i > 0 && i.Epic).Count());
                else if (RelicFlag.Count() >= 2)
                    proto.Relic = (byte)(39 + relic.RelicAttributes.Where(i => i > 0 && i.Epic).Count());
                else
                    proto.Relic = (byte)(42 + relic.RelicAttributes.Where(i => i > 0 && i.Epic).Count());
            }
            proto.SageMode1 = NiniaP0;

            if (Owner.Player.CollectionID != 0)
                proto.PistilAroma = Owner.Player.CollectionID;

            if (Owner.Player.PandaID != 0)
                proto.PandaPonpon = Owner.Player.PandaID;
            proto.HaloTitle = WingTitleHaloHide ? 0 : Owner.Player.SpecialHaloID;

            proto.StatusFlags = new ulong[BitVector.bits.Length];
            proto.Medal = Medal;
            for (int x = 0; x < BitVector.bits.Length; x++)
                proto.StatusFlags[x] = BitVector.bits[x];
            proto.HundredWeapons = new long[5];
            var visibleHundredWeapons = Owner.HundredWeapons.Objects.Values.Where(i => i.AppearancePosition > 0).OrderBy(i => i.AppearancePosition).ToArray();
            for (int i = 0; i < Math.Min(proto.HundredWeapons.Length, visibleHundredWeapons.Length); i++)
            {
                var binaryFormat = Convert.ToString((int)visibleHundredWeapons[i].WeaponSubtype, 2) + Convert.ToString(visibleHundredWeapons[i].Score, 2);
                while (binaryFormat.Length < 25)
                    binaryFormat = binaryFormat + "0";
                proto.HundredWeapons[i] = Convert.ToUInt32(binaryFormat, 2);
            }
            proto.MountArmorColor = MountArmorColor;
            proto.FrameID = Owner.Player.FrameID;
            proto.todayFlowerType = TodayFlowerType;
            proto.FlowerRank = (ulong)FlowerRank;
            proto.MachineInfo = Owner.Player.TableBetDice;
            if (AtributesStatus.IsWarrior(this.Class))
            {
                Archives.Item obj = this.Owner.MyArchives.isOpen();
                if (obj != null)
                {
                    proto.HundredWeapons[1] = (uint)obj.ItemID * 10 + obj.dwParam;
                    proto.HundredWeapons[2] = obj.Progress;
                }
            }
            if (AtributesStatus.IsArcher(this.Class))
            {
                Archives.Item obj = this.Owner.MyArchives.isOpen();
                if (obj != null)
                {
                    proto.HundredWeapons[1] = (uint)obj.ItemID;
                    proto.HundredWeapons[2] = obj.Progress;
                }
            }
            if (AtributesStatus.IsMonk(this.Class))
            {
                Archives.Item obj = this.Owner.MyArchives.isOpen();
                if (obj != null)
                {
                    proto.HundredWeapons[1] = (uint)obj.ItemID;
                    proto.HundredWeapons[2] = obj.Progress;
                }
            }
            if (AtributesStatus.IsTaoist(this.Class))
            {
                Archives.Item t = this.Owner.MyArchives.isOpen();
                if (t != null)
                {
                    proto.HundredWeapons = new long[4];
                    proto.HundredWeapons[0] = (uint)(t.ItemID);
                    proto.HundredWeapons[1] = t.Progress;
                    if (this.TaoHeavenState > 0)
                        proto.HundredWeapons[2] = this.Owner.HeavenVector;
                    else
                        proto.HundredWeapons[2] = 31;

                }
            }
            if (AtributesStatus.IsPirate(this.Class))
            {
                Archives.Item obj = this.Owner.MyArchives.isOpen();
                if (obj != null)
                {
                    proto.HundredWeapons[0] = (uint)(obj.ItemID);
                    proto.HundredWeapons[2] = obj.Progress;

                }
            }
            if (AtributesStatus.IsLee(this.Class))
            {
                Archives.Item obj = this.Owner.MyArchives.isOpen();
                if (obj != null)
                {
                    proto.HundredWeapons[0] = (uint)(obj.ItemID);
                    proto.HundredWeapons[1] = obj.Progress;
                    proto.HundredWeapons[3] = (long)Owner.MyArchives.Effect;
                }
            }
            proto.FootPrintID = SpecialFootprintID;
            proto.ActionCool = SpecialHaloAction;
            proto.FootPrintScore = SpecialFootprintScore;
            proto.ActionCoolScore = SpecialHaloActionScore;
            stream.ProtoBufferSerialize(proto);
            stream.Finalize(Game.GamePackets.MsgPlayer);
            return stream;
        }

        public byte MysteryFruit = 0;
        public uint GetShareBattlePowers(uint target_battlepower)
        {
            return (uint)Database.TutorInfo.ShareBattle(this.Owner, (int)target_battlepower);
        }
        public uint TableID { get; set; }
        public uint PokerTableID { get; set; }

        public byte PokerSeat { get; set; }
        public byte VotePoints = 0;

        public byte GuildBoss;
        public uint NobilityCps = 0;
        public Qahr.Mobile.Game.MsgServer.MsgSpell FocusClientSpell;
        public Qahr.Mobile.Game.MsgServer.MsgSpell WeepStorm;
        public bool doFocus = false;
        public int getFan(bool Magic)
        {
            if (Owner.Equipment.FreeEquip(Role.Flags.ConquerItem.Fan))
                return 0;

            ushort magic = 0;
            ushort physical = 0;
            ushort gemVal = 0;

            #region Get

            MsgGameItem Item = this.Owner.Equipment.TryGetEquip(Flags.ConquerItem.Fan);

            if (Item != null)
            {
                if (Item.ITEM_ID > 0)
                {
                    switch (Item.ITEM_ID % 10)
                    {
                        case 3:
                        case 4:
                        case 5: physical += 300; magic += 150; break;
                        case 6: physical += 500; magic += 200; break;
                        case 7: physical += 700; magic += 300; break;
                        case 8: physical += 900; magic += 450; break;
                        case 9: physical += 1200; magic += 750; break;
                    }

                    switch (Item.Plus)
                    {
                        case 0: break;
                        case 1: physical += 200; magic += 100; break;
                        case 2: physical += 400; magic += 200; break;
                        case 3: physical += 600; magic += 300; break;
                        case 4: physical += 800; magic += 400; break;
                        case 5: physical += 1000; magic += 500; break;
                        case 6: physical += 1200; magic += 600; break;
                        case 7: physical += 1300; magic += 700; break;
                        case 8: physical += 1400; magic += 800; break;
                        case 9: physical += 1500; magic += 900; break;
                        case 10: physical += 1600; magic += 950; break;
                        case 11: physical += 1700; magic += 1000; break;
                        case 12: physical += 1800; magic += 1050; break;
                    }
                    switch (Item.SocketOne)
                    {
                        case Flags.Gem.NormalThunderGem: gemVal += 100; break;
                        case Flags.Gem.RefinedThunderGem: gemVal += 300; break;
                        case Flags.Gem.SuperThunderGem: gemVal += 500; break;
                    }
                    switch (Item.SocketTwo)
                    {
                        case Flags.Gem.NormalThunderGem: gemVal += 100; break;
                        case Flags.Gem.RefinedThunderGem: gemVal += 300; break;
                        case Flags.Gem.SuperThunderGem: gemVal += 500; break;
                    }
                }
            }

            #endregion Get

            magic += gemVal;
            physical += gemVal;

            if (Magic)
                return (int)magic;
            else
                return (int)physical;
        }





        public int SubstitutionDefence = 0;

        public uint MaxDecreaseHP = 0;

        public Time32 CrescentShadow;



        public uint SoulRate = 0;

        public byte BWindowsView;

        public uint MountArmorColor;

        public bool CanClaim = false;
        public DateTime lastJumpTime = new DateTime();

        public int ChaoticDance { get; set; }

        public int SuperSpeedGun { get; set; }

        public string DonatePoint { get; set; }






    }

}