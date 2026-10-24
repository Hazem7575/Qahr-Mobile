using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Qahr.Mobile.Structures;
using Qahr.Mobile.Structures.Interfaces;
using Qahr.Mobile.Game.MsgServer;
using System.IO;
using Qahr.Mobile.DBFunctionality;
// [net10] MySqlConnector in the .NET 10 build (Qahr.Mobile.Net10.csproj); MySql.Data in the 4.8 build.
using MySqlConnector;
using MySqlClientNs = MySqlConnector;
using Qahr.Mobile.Game;
using Qahr.Mobile.MY_System.Relic;
using System.Collections.Concurrent;

namespace Qahr.Mobile.Client
{
    [Flags]
    public enum ServerFlag : ushort
    {
        None = 0,
        AcceptLogin = 1 << 0,
        CreateCharacter = 1 << 1,
        CreateCharacterSucces = 1 << 2,
        LoginFull = 1 << 3,
        SetLocation = 1 << 4,
        OnLoggion = 1 << 5,
        QueuesSave = 1 << 6,
        RemoveSpouse = 1 << 7,
        Disconnect = 1 << 8,
        UpdateSpouse = 1 << 9
    }
    public unsafe class GameClient
    {


        public uint LastClientPing { get; set; }
        public DateTime LastPingTime { get; set; }

        public object JumpLock = new object();
        public DateTime PingPacket = DateTime.Now;
        public DateTime SendCheckStamp = DateTime.Now;
        public DateTime LastPingPacket = DateTime.Now;
        public DateTime MemoryCheckStamp = DateTime.Now;
        public DateTime ProcessesCheckStamp = DateTime.Now;
        public int firstchar;

        public int secondchar;
        public int finalchar;
        public int ModeAtributes;
        public bool FullLoading = false;
        public uint EnemyInvadeId;
        public Qahr.Mobile.Role.Instance.CoatColorRule CoatColorRule;

        //public HeroGathering.User EnemyInvade;
        //public BotAttack MyBot;
        public MedalStorage MedalStorage;
        public MainRelic MainRelic;
        public ConcurrentDictionary<uint, MsgGameItem> MinorRelics = new ConcurrentDictionary<uint, MsgGameItem>();
        public System.SafeDictionary<uint, MsgGameItem> MinorRilec = new System.SafeDictionary<uint, MsgGameItem>();

        public void AddMinorRelic(MsgGameItem item)
        {
            if (item == null)
                return;

            MinorRelics[item.UID] = item;
            MinorRilec[item.UID] = item;
        }

        public unsafe void SyncMinorRelicBag()
        {
            using (var rec = new ServerSockets.RecycledPacket())
            {
                var stream = rec.GetStream();
                foreach (var relic in MinorRilec.Values)
                    Send(stream.ItemViewCreate(Player.UID, 0, relic, MsgItemView.ActionMode.RelicFuseNew));
            }
        }
        //public System.SafeDictionary<uint, MsgGameItem> RelicFuseNew = new System.SafeDictionary<uint, MsgGameItem>();
        public uint RelicCount = 0;
        public uint RelicBoostDone = 0;
        #region AllDictionary

        public System.Collections.Concurrent.ConcurrentDictionary<uint, MsgGameItem> MythSoulBag = new System.Collections.Concurrent.ConcurrentDictionary<uint, MsgGameItem>();

        public System.SafeDictionary<uint, PrizeInfo> PrizeInfo = new System.SafeDictionary<uint, PrizeInfo>();


        public System.Collections.Concurrent.ConcurrentDictionary<Role.Instance.RoleStatus.StatuTyp, Role.Instance.RoleStatus> ExtraStatus;
        #endregion
        public uint StampThreadMemory, StampThreadTimer;
        public DateTime LoaderTime = DateTime.Now;
        public uint StampThread = 0;
        public bool TerminateLoader = false;
        public bool ActiveClient = false;
        public List<string> OpenedProcesses = new List<string>();

        public void TeleportTC()
        {
            Teleport(Places.TwinCity, 0, true, false);


        }

        #region QuizShow

        public int QuizRank = 0;

        public int GetQuizTimer()
        {
            TimeSpan now = new TimeSpan(DateTime.Now.Ticks);
            TimeSpan old = new TimeSpan(StartQuizTimer.Ticks);
            return (int)(now.TotalSeconds - old.TotalSeconds);
        }

        public ushort QuizShowPoints = 0;
        #endregion

        #region InfoPrestrige

        public uint PrestigeLevel;

        public uint MyPrestigePoints = 0;

        public Database.PrestigeRanking.Entry PrestrigeEntry;

        public uint[] PrestigePoints = new uint[66];

        //public static bool UpdateSpeedFlags(Client.GameClient client)
        //{

        //    {
        //        return true;
        //    }
        //    return false;
        //}
        public void CreatePrestigePoints()
        {
            if (Fake)
                return;

            PrestigePoints = new uint[PrestigePoints.Length];

            #region MyJiangHu[0]
            if (Player.MyJiangHu != null)
            {
                if (Player.MyJiangHu.Inner_Strength > 0 && Player.MyJiangHu.Inner_Strength <= 16200)
                    PrestigePoints[0] = (uint)((Player.MyJiangHu.Inner_Strength / 100) * 30);
                else if (Player.MyJiangHu.Inner_Strength >= 16201 && Player.MyJiangHu.Inner_Strength <= 40500)
                    PrestigePoints[0] = (uint)((Player.MyJiangHu.Inner_Strength / 100) * 33);
                else if (Player.MyJiangHu.Inner_Strength >= 40501 && Player.MyJiangHu.Inner_Strength <= 60750)
                    PrestigePoints[0] = (uint)((Player.MyJiangHu.Inner_Strength / 100) * 36);
                else if (Player.MyJiangHu.Inner_Strength >= 60751 && Player.MyJiangHu.Inner_Strength <= 72000)
                    PrestigePoints[0] = (uint)((Player.MyJiangHu.Inner_Strength / 100) * 40);
                else if (Player.MyJiangHu.Inner_Strength >= 72001 && Player.MyJiangHu.Inner_Strength <= 79200)
                    PrestigePoints[0] = (uint)((Player.MyJiangHu.Inner_Strength / 100) * 45);
                else if (Player.MyJiangHu.Inner_Strength >= 79201 && Player.MyJiangHu.Inner_Strength <= 80800)
                    PrestigePoints[0] = (uint)((Player.MyJiangHu.Inner_Strength / 100) * 50);
                else if (Player.MyJiangHu.Inner_Strength >= 80801 && Player.MyJiangHu.Inner_Strength <= 81000)
                    PrestigePoints[0] = (uint)((Player.MyJiangHu.Inner_Strength / 100) * 60);
            }
            #endregion

            #region MyChi[1]
            if (Player.MyChi != null)
            {
                foreach (var obj in Player.MyChi)
                {
                    foreach (var att in obj.Attributes)
                    {
                        if (att.Value == 0) continue;
                        if (obj.Score <= 276)
                            PrestigePoints[1] += 25 * (uint)((att.Value - Role.Instance.Chi.ChiMinValues(att.Type)) / (double)((Role.Instance.Chi.ChiMaxValues(att.Type) - Role.Instance.Chi.ChiMinValues(att.Type)) / 100d));
                        else if (obj.Score <= 316)
                            PrestigePoints[1] += 30 * (uint)((att.Value - Role.Instance.Chi.ChiMinValues(att.Type)) / (double)((Role.Instance.Chi.ChiMaxValues(att.Type) - Role.Instance.Chi.ChiMinValues(att.Type)) / 100d));
                        else if (obj.Score <= 356)
                            PrestigePoints[1] += 35 * (uint)((att.Value - Role.Instance.Chi.ChiMinValues(att.Type)) / (double)((Role.Instance.Chi.ChiMaxValues(att.Type) - Role.Instance.Chi.ChiMinValues(att.Type)) / 100d));
                        else if (obj.Score <= 380)
                            PrestigePoints[1] += 40 * (uint)((att.Value - Role.Instance.Chi.ChiMinValues(att.Type)) / (double)((Role.Instance.Chi.ChiMaxValues(att.Type) - Role.Instance.Chi.ChiMinValues(att.Type)) / 100d));
                        else if (obj.Score <= 396)
                            PrestigePoints[1] += 45 * (uint)((att.Value - Role.Instance.Chi.ChiMinValues(att.Type)) / (double)((Role.Instance.Chi.ChiMaxValues(att.Type) - Role.Instance.Chi.ChiMinValues(att.Type)) / 100d));
                        else if (obj.Score <= 399)
                            PrestigePoints[1] += 50 * (uint)((att.Value - Role.Instance.Chi.ChiMinValues(att.Type)) / (double)((Role.Instance.Chi.ChiMaxValues(att.Type) - Role.Instance.Chi.ChiMinValues(att.Type)) / 100d));
                        else
                            PrestigePoints[1] += 60 * (uint)((att.Value - Role.Instance.Chi.ChiMinValues(att.Type)) / (double)((Role.Instance.Chi.ChiMaxValues(att.Type) - Role.Instance.Chi.ChiMinValues(att.Type)) / 100d));
                    }
                }
            }
            #endregion

            #region InnerPower[2]
            if (Player.InnerPower != null)
            {
                foreach (var obj in Player.InnerPower.Stages)
                    if (obj.UnLocked)
                        PrestigePoints[2] += 100;
            }
            #endregion

            #region Equipment[3]
            foreach (var item in Equipment.CurentEquip)
                PrestigePoints[3] += item.ItemPoints;

            #endregion

            #region Level[4]
            if (Player.Level < 120)
                PrestigePoints[4] += (uint)Player.Level * 10;
            else if (Player.Level >= 120 && Player.Level < 130)
                PrestigePoints[4] += (uint)Player.Level * 15;
            else if (Player.Level >= 130 && Player.Level < 140)
                PrestigePoints[4] += (uint)Player.Level * 20;
            else
                PrestigePoints[4] += (uint)Player.Level * 25;
            #endregion

            #region Player.Strength + Player.Spirit + Player.Vitality + Player.Agility[6]
            PrestigePoints[6] = (uint)((Player.Strength + Player.Spirit + Player.Vitality + Player.Agility) * 5);
            #endregion

            #region Reborn[7]
            PrestigePoints[7] = (uint)Player.Reborn * 1000;
            #endregion

            #region SubClass[15]
            if (Player.SubClass != null)
            {
                foreach (var obj in Player.SubClass.src.Values)
                {
                    PrestigePoints[15] += (uint)obj.Level * 80;
                    PrestigePoints[15] += (uint)obj.Phrase * 20;
                }
            }
            #endregion

            #region MyWardrobe Garment [16]
            PrestigePoints[16] = (uint)(MyWardrobe.Items[(uint)Role.Instance.Wardrobe.ItemsType.Garment].Count * 50);
            #endregion

            #region MyWardrobe Mount[17]
            PrestigePoints[17] = (uint)(MyWardrobe.Items[(uint)Role.Instance.Wardrobe.ItemsType.Mount].Count * 50);
            #endregion

            #region Nobility[18]
            PrestigePoints[18] = (uint)Player.NobilityRank * 1000;
            #endregion

            #region PerfectionRedRune[20]
            PrestigePoints[20] = Rune.PerfectionRedRune;
            #endregion

            #region PerfectionBlueRune[21]
            PrestigePoints[21] = Rune.PerfectionBlueRune;
            #endregion

            #region PerfectionYellowRune[22]
            PrestigePoints[22] = Rune.PerfectionYellowRune;
            #endregion
            #region PerfectionIdelRune[22]
            uint PerfectionIdelRune = Rune.PerfectionIdelRune;
            PrestigePoints[22] += PerfectionIdelRune;
            #endregion



            #region Beasts[37]
            if (Beasts.Activated)
            {
                PrestigePoints[37] = 300;
                for (byte i = 0; i < Beasts.Level; i++)
                    PrestigePoints[37] += (uint)(i >= 50 ? 100 : 200);
            }
            #endregion

            #region ArchivesTrojan[38]
            if (Database.AtributesStatus.IsTrojan(Player.Class))
                PrestigePoints[38] = HundredWeapons.PerfectionScore;
            else
                PrestigePoints[38] = ((HundredWeapons.PerfectionScorePrecent) * 15) / 100;
            #endregion

            #region Promotion[39]
            if (Database.ProfessionTable.Benefits.ContainsKey(Player.Class))
                PrestigePoints[39] = Database.ProfessionTable.Benefits[Player.Class].PrestigeScore;
            #endregion

            #region ArchivesNinja[43]

            if (MyNinja != null)
            {
                if (Database.AtributesStatus.IsNinja(Player.Class))
                {
                    PrestigePoints[43] = (MyNinja.PerfectionScore * 50) / 100;
                }
                else
                {
                    uint newscore = (MyNinja.PerfectionScore * 50) / 100;
                    PrestigePoints[43] = newscore * 15 / 100;
                }
            }

            #endregion

            #region ArchivesWarrior[44]

            if (MyArchives != null)
            {
                if (Database.AtributesStatus.IsWarrior(Player.Class))
                {
                    uint AllScore = 0;
                    var items = MyArchives.Items.Values.Where(p => p.ItemID >= Role.Instance.Archives.TypeID.Dragonhowl && p.ItemID <= Role.Instance.Archives.TypeID.Redcurse);
                    foreach (var Score in items)
                    {
                        AllScore += Score.DBItem.Score;
                    }
                    PrestigePoints[44] += (AllScore * 15) / 100;
                }
                else
                {
                    uint AllScore = 0;
                    var items = MyArchives.Items.Values.Where(p => p.ItemID >= Role.Instance.Archives.TypeID.Dragonhowl && p.ItemID <= Role.Instance.Archives.TypeID.Redcurse);
                    foreach (var Score in items)
                    {
                        AllScore += Score.DBItem.Score;
                    }
                    uint newscore = (AllScore * 15) / 100;
                    PrestigePoints[44] = newscore * 15 / 100;
                }
            }

            #endregion

            #region ArchivesArcher[55]
            if (MyArchives != null)
            {
                if (Database.AtributesStatus.IsArcher(Player.Class))
                {
                    uint AllScore = 0;
                    var items = MyArchives.Items.Values.Where(p => p.ItemID >= Role.Instance.Archives.TypeID.StoneCracker && p.ItemID <= Role.Instance.Archives.TypeID.ThornCutter);
                    foreach (var Score in items)
                    {
                        AllScore += Score.DBItem.Score;
                    }

                    PrestigePoints[55] += (AllScore * 15) / 100;
                }
                else
                {
                    uint AllScore = 0;
                    var items = MyArchives.Items.Values.Where(p => p.ItemID >= Role.Instance.Archives.TypeID.StoneCracker && p.ItemID <= Role.Instance.Archives.TypeID.ThornCutter);
                    foreach (var Score in items)
                    {
                        AllScore += Score.DBItem.Score;
                    }
                    uint newscore = (AllScore * 15) / 100;
                    PrestigePoints[55] = newscore * 15 / 100;
                }
            }

            #endregion

            #region MythSoulScore[47]
            foreach (var item in this.MythSoulBag.Values.ToArray())
            {
                Database.MythTable.MythSoulEXP DBitem = null;
                DBitem = Database.MythTable.MythSoulExpList.Values.Where(P => P.ItemType == item.ITEM_ID).FirstOrDefault();
                if (DBitem != null)
                {
                    PrestigePoints[47] += DBitem.Score;
                }

            }
            #endregion

            #region Collection[49]
            if (this.Collection.items != null)
            {
                for (int x = 0; x < Collection.items.Count; x++)
                {
                    PrestigePoints[49] += 500;
                }

            }
            #endregion

            #region ArchivesTaoist[51]

            if (MyArchives != null)
            {
                if (Database.AtributesStatus.IsTaoist(Player.Class))
                {
                    uint AllScore = 0;
                    var items = MyArchives.Items.Values.Where(p => p.ItemID >= Role.Instance.Archives.TypeID.Vicissitude && p.ItemID <= Role.Instance.Archives.TypeID.BirthDeath);
                    foreach (var Score in items)
                    {
                        AllScore += Score.DBItem.Score;
                    }
                    foreach (var Scores in MyArchives.JadeBag.Values)
                    {
                        AllScore += Scores.DBJadeList.score;
                    }
                    PrestigePoints[51] += (AllScore * 15) / 100;
                }
                else
                {
                    uint AllScore = 0;
                    var items = MyArchives.Items.Values.Where(p => p.ItemID >= Role.Instance.Archives.TypeID.Vicissitude && p.ItemID <= Role.Instance.Archives.TypeID.BirthDeath);
                    foreach (var Score in items)
                    {
                        AllScore += Score.DBItem.Score;
                    }
                    foreach (var Scores in MyArchives.JadeBag.Values)
                    {
                        AllScore += Scores.DBJadeList.score;
                    }
                    uint newscore = (AllScore * 15) / 100;
                    PrestigePoints[51] = newscore * 15 / 100;
                }
            }

            #endregion

            #region ArchivesPirate[52]
            if (MyArchives != null)
            {
                uint AllScore = 0;
                if (Database.AtributesStatus.IsPirate(Player.Class))
                {
                    var items = MyArchives.Items.Values.Where(p => p.ItemID >= Role.Instance.Archives.TypeID.ThunderNut && p.ItemID <= Role.Instance.Archives.TypeID.LavaNut);
                    foreach (var Score in items)
                    {
                        AllScore += Score.DBItem.Score;
                    }
                    foreach (var Scores in MyArchives.JadeBag.Values)
                    {
                        AllScore += Scores.DBJadeList.score;
                    }
                    PrestigePoints[52] += (AllScore * 15) / 100;
                }
                else
                {
                    var items = MyArchives.Items.Values.Where(p => p.ItemID >= Role.Instance.Archives.TypeID.ThunderNut && p.ItemID <= Role.Instance.Archives.TypeID.LavaNut);
                    foreach (var Score in items)
                    {
                        AllScore += Score.DBItem.Score;
                    }
                    foreach (var Scores in MyArchives.JadeBag.Values)
                    {
                        AllScore += Scores.DBJadeList.score;
                    }
                    uint newscore = (AllScore * 15) / 100;
                    PrestigePoints[52] = newscore * 15 / 100;
                }
            }

            #endregion

            #region ArchivesIsLee[59]

            if (MyArchives != null)
            {
                uint AllScore = 0;
                if (Database.AtributesStatus.IsLee(Player.Class))
                {
                    var items = MyArchives.Items.Values.Where(p => p.ItemID >= Role.Instance.Archives.TypeID.Dragon && p.ItemID <= Role.Instance.Archives.TypeID.Suanni);
                    foreach (var Score in items)
                    {

                        AllScore += Score.DBItem.Score;
                    }
                    foreach (var Scores in MyArchives.JadeBag.Values)
                    {
                        AllScore += Scores.DBJadeList.score;
                    }
                    PrestigePoints[59] += (AllScore * 15) / 100;
                }
                else
                {
                    var items = MyArchives.Items.Values.Where(p => p.ItemID >= Role.Instance.Archives.TypeID.Dragon && p.ItemID <= Role.Instance.Archives.TypeID.Suanni);
                    foreach (var Score in items)
                    {

                        AllScore += Score.DBItem.Score;
                    }
                    foreach (var Scores in MyArchives.JadeBag.Values)
                    {
                        AllScore += Scores.DBJadeList.score;
                    }
                    uint newscore = (AllScore * 15) / 100;
                    PrestigePoints[59] = newscore * 15 / 100;
                }
            }

            #endregion


            if (this.Player.VipLevel >= 7)
            {
                MyPrestigePoints += 20000;
            }


            #region Astredge System [63]
            if (MyAstredge != null && MyAstredge.Items != null)
            {
                uint prestige = 0;

                foreach (var item in MyAstredge.Items.Values)
                {
                    if (item != null && item.DBItem != null)
                    {
                        prestige += item.DBItem.Prestige;
                    }
                }

                PrestigePoints[63] = prestige;
            }
            #endregion


            #region Eonspirit System [65]
            if (EonspiritSystem != null && EonspiritSystem.Count > 0)
            {
                uint exp = 0;

                foreach (var item in EonspiritSystem.Values)
                {
                    if (item.Position != (ushort)Qahr.Mobile.Role.Enums.EonspiritPosition.EonspiritActive &&
                        item.Position != (ushort)Qahr.Mobile.Role.Enums.EonspiritPosition.EonspiritUnActive)
                        continue;

                    var info = Database.YuanshenLevUP.YuanshenLevUPItem.Values
                        .FirstOrDefault(i =>
                            i.Level == item.ITEM_ID % 100 &&
                            i.TypeLevel == item.EonspiritPercentage);

                    if (info != null)
                    {
                        exp += info.Rating / 1;
                    }
                }

                PrestigePoints[65] = exp;
            }
            #endregion


            MyPrestigePoints = 0;

            for (int x = 0; x < PrestigePoints.Length; x++)
                MyPrestigePoints += PrestigePoints[x];

            PrestrigeEntry = new Database.PrestigeRanking.Entry();
            PrestrigeEntry.type = Database.PrestigeRanking.GetIndex(Player.Class);
            PrestrigeEntry.UID = Player.UID;
            PrestrigeEntry.Name = Player.Name;
            PrestrigeEntry.Class = Player.Class;
            PrestrigeEntry.TotalPoints = MyPrestigePoints;
            PrestrigeEntry.Level = (byte)Player.Level;
            PrestrigeEntry.Mesh = Player.Mesh;
            PrestrigeEntry.AddInfo(this);
            PrestrigeEntry.Points = new uint[PrestigePoints.Length];
            for (int x = 0; x < PrestigePoints.Length; x++)
                PrestrigeEntry.Points[x] = PrestigePoints[x];
            if (!Fake)
            {
                if (Database.GroupServerList.MyServerInfo.ID == Player.ServerID)
                {
                    if (!Player.Name.Contains("[Bot]") && !Player.Name.Contains("[GM]") && !Player.Name.Contains("[PM]") && !Player.Name.Contains("[TQ]"))
                    {
                        if (Player.Class < 10000 || Player.Class > 10002)
                            Database.PrestigeRanking.Ranks[PrestrigeEntry.type].AddItem(PrestrigeEntry);
                        Database.PrestigeRanking.Ranks[Database.PrestigeRanking.Type.World].AddItem(PrestrigeEntry);
                    }
                }
            }
            PerfectionStatus.Update((int)PrestigeLevel, this);
            if (Player.MyGuild != null && Player.MyGuildMember != null)
                Player.MyGuildMember.PrestigePoints = PrestrigeEntry.TotalPoints;
            using (var rec = new ServerSockets.RecycledPacket())
            {
                var stream = rec.GetStream();
                var info = new MsgUserAbilityScore.UserAbilityScore();
                info.type = 1;
                info.Level = this.Player.Level;
                info.UID = this.Player.UID;
                info.Items = new MsgUserAbilityScore.AbilityScore[PrestigePoints.Length];
                for (int x = 0; x < PrestigePoints.Length; x++)
                {
                    info.Items[x] = new MsgUserAbilityScore.AbilityScore();
                    info.Items[x].Position = (ushort)(x + 1);
                    info.Items[x].Points = PrestigePoints[x];
                }
                this.Send(stream.UserAbilityScoreCreate(info));
            }
            Database.PrestigeRanking.UpdateRank(this);
        }

        public void UpdatePerfectionLevel(ServerSockets.Packet stream)
        {
            Database.TitleStorage.CheckUpUser(this, stream);
            MsgUserTotalRefineLev.TotalRefineLev msg = new MsgUserTotalRefineLev.TotalRefineLev();
            msg.Level = PrestigeLevel;
            msg.UID = Player.UID;
            msg.Type = 0;
            Send(stream.UserTotalRefineLevCreate(msg));

        }
        #endregion

        #region CrossServer
        public MsgInterServer.PipeServer.User PipeServer;

        public MsgInterServer.PipeClient PipeClient = null;

        public bool IsConnectedInterServer() { return PipeClient != null; }

        public Cryptography.DiffieHellman DHKey;
        #endregion


        public Extensions.Time32 FloorSpellStamp = Extensions.Time32.Now.AddMilliseconds(ThreadPlayer.User_FloorSpell);

        public Extensions.Time32 BuffersStamp = Extensions.Time32.Now.AddMilliseconds(ThreadPlayer.User_Buffers);

        public Extensions.Time32 StaminStamp = Extensions.Time32.Now.AddMilliseconds(ThreadPlayer.User_Stamina);

        public Extensions.Time32 AttackStamp = Extensions.Time32.Now.AddMilliseconds(ThreadPlayer.User_AutoAttack);

        public Extensions.Time32 XPCountStamp = Extensions.Time32.Now.AddMilliseconds(ThreadPlayer.User_StampXPCount);

        public Extensions.Time32 CheckSecoundsStamp = Extensions.Time32.Now.AddMilliseconds(ThreadPlayer.User_CheckSecounds);

        public Extensions.Time32 CheckItemTimeStamp = Extensions.Time32.Now.AddMilliseconds(ThreadPlayer.User_ItemTIme);

        #region DataTimePlayer

        public DateTime StampMainPlayerThread = DateTime.Now;

        public DateTime CheckItemSecs = DateTime.Now;

        public DateTime StampAutoAttack = DateTime.Now;

        public DateTime StampMonsterThread = DateTime.Now;

        public IDisposable[] TimerSubscriptions;
        public object TimerSyncRoot;

        public DateTime StartQuizTimer = new DateTime();

        public DateTime LastVIPTeleport = new DateTime();

        public DateTime LastVIPTeamTeleport = new DateTime();
        #endregion

        #region Arena&&ArenaTeam&&ElitePk&&SkillTeamPK

        public Game.MsgTournaments.MsgArena.User ArenaStatistic;

        public Game.MsgTournaments.MsgArena.Match ArenaMatch;

        public Game.MsgTournaments.MsgArena.Match ArenaWatchingGroup;

        public Game.MsgTournaments.MsgTeamEliteGroup.Match TeamElitePkWatchingGroup;

        public Game.MsgTournaments.MsgEliteGroup.FighterStats ElitePKStats;

        Game.MsgTournaments.MsgEliteGroup.Match _tet;

        public Game.MsgTournaments.MsgEliteGroup.Match ElitePkMatch
        {

            get { return _tet; }
            set
            {
                _tet = value;
            }
        }

        public Game.MsgTournaments.MsgEliteGroup.Match ElitePkWatchingGroup;


        public bool InSkillTeamPk()
        {
            return Team != null && Team.PkMatch != null && Team.PkMatch.elitepkgroup.PKTournamentID == Game.GamePackets.MsgTeamPopPKArenic && Player.InTeamPk;
        }

        public uint ArenaPoints
        {
            get
            {
                if (ArenaStatistic == null)
                    return 0;
                return ArenaStatistic.Info.ArenaPoints;
            }
            set
            {
                if (ArenaStatistic != null)
                    ArenaStatistic.Info.ArenaPoints = value;
            }
        }





        public uint ArenaHonorPoints
        {
            get
            {
                if (ArenaStatistic == null)
                    return 0;
                return ArenaStatistic.Info.CurrentHonor;
            }
            set
            {
                if (ArenaStatistic != null)
                    ArenaStatistic.Info.CurrentHonor = value;
            }
        }

        internal bool IsWatching()
        {
            return ArenaWatchingGroup != null || ElitePkWatchingGroup != null || TeamElitePkWatchingGroup != null;
        }

        internal bool InQualifier()
        {
            if (global::Qahr.Mobile.TeamQualifier.TeamArenaCore.InMatch(this)) return true; // [TeamArena 2026-10-14]
            return
                ArenaStatistic.ArenaState != Game.MsgTournaments.MsgArena.User.StateType.None && ArenaMatch != null

                || (ElitePkMatch != null)
                || (Team != null && Team.PkMatch != null);
        }

        internal bool InTeamQualifier()
        {
            return Team != null && (Team.PkMatch != null);
        }

        internal void EndQualifier()
        {
            if (ArenaMatch != null)
                ArenaMatch.End(this);
            //if (EnemyInvade.GET_Match != null)
            //    EnemyInvade.GET_Match.End(this);

            if (ElitePkMatch != null)
            {
                ElitePkMatch.End(this, true);
            }
            if (Team != null)
            {
                if (Team.PkMatch != null)
                {
                    if (Team.TeamLider(this))
                    {
                        if (Team.Members.Count <= 1)
                        {
                            Team.PkMatch.End(this.Team, true);
                            return;
                        }
                    }
                    if (Team.IsDead(9855))
                        Team.PkMatch.End(this.Team, true);
                }
            }
        }

        internal void UpdateQualifier(GameClient client, GameClient target, uint damage)
        {
            if (client.Player.Map == 9855)
            {
                //if (EnemyInvade.GET_Match != null)
                //{
                //    EnemyInvade.Damage += damage;
                //    EnemyInvade.GET_Match.SendScore();
                //}
                if (ArenaMatch != null)
                {
                    client.ArenaStatistic.Damage += damage;
                    ArenaMatch.SendScore();
                }
                if (Team != null)
                {

                    if (Team.PkMatch != null)
                    {
                        Team.PKStats.Points += damage;
                        Team.PkMatch.UpdateScore();
                    }
                }
                if (ElitePKStats != null && ElitePkMatch != null)
                {
                    ElitePKStats.Points += damage;
                    ElitePkMatch.UpdateScore();
                }
            }
        }
        #endregion

        #region SpellRange&&SpellsSoul
        public bool AllowUseSpellOnSteed(ushort Spell)
        {
            if (!Player.ContainFlag(Game.MsgServer.MsgUpdate.Flags.Ride))
                return true;
            if (Equipment.RidingCrop != 0)
                return true;
            else if (Spell == (ushort)Role.Flags.SpellID.Spook || Spell == (ushort)Role.Flags.SpellID.WarCry || Spell == (ushort)Role.Flags.SpellIDPirate.LordThreat || Spell == (ushort)Role.Flags.SpellIDPirate.LordThreatPassive
                || Spell == (ushort)Role.Flags.SpellID.Riding)
                return true;
            return false;
        }

        public bool IsInSpellRange(uint UID, int range)
        {
            Role.IMapObj target;
            if (Player.View.TryGetValue(UID, out target, Role.MapObjectType.Monster))
            {
                return Role.Core.GetDistance(Player.X, Player.Y, target.X, target.Y) <= range;
            }
            else if (Player.View.TryGetValue(UID, out target, Role.MapObjectType.Player))
            {
                return Role.Core.GetDistance(Player.X, Player.Y, target.X, target.Y) <= range;
            }
            else if (Player.View.TryGetValue(UID, out target, Role.MapObjectType.SobNpc))
            {
                return Role.Core.GetDistance(Player.X, Player.Y, target.X, target.Y) <= range;
            }
            return false;
        }

        unsafe internal bool UpdateSpellSoul(ServerSockets.Packet stream, Role.Flags.SpellID SpellID, byte MaxLevel)
        {
            Game.MsgServer.MsgSpell spell;
            if (MySpells.ClientSpells.TryGetValue((ushort)SpellID, out spell))
            {
                if (spell.SoulLevel >= MaxLevel)
                {

                    CreateBoxDialog("Sorry, you spell " + SpellID.ToString() + " is max level.");
                    return false;
                }

                ActionQuery action = new ActionQuery()
                {
                    ObjId = Player.UID,
                    dwParam = (ushort)SpellID,
                    Type = ActionType.RemoveSpell
                };
                Send(stream.ActionCreate(action));

                spell.SoulLevel++;
                spell.UseSpellSoul = spell.SoulLevel;

                Send(stream.SpellCreate(spell));

                return true;
            }
            else
            {

                CreateBoxDialog("Sorry, you not have the spell " + SpellID.ToString() + ".");

                return false;
            }
        }
        #endregion

        #region Roulette
        public uint PlayRouletteUID = 0;

        public uint WatchRoulette = 0;

        public void CheckRouletteDisconnect()
        {
            Database.Roulettes.RouletteTable Table;
            if (PlayRouletteUID != 0)
            {
                if (Database.Roulettes.RoulettesPoll.TryGetValue(PlayRouletteUID, out Table))
                {
                    Table.RemovePlayer(this);
                }
            }
            else if (WatchRoulette != 0)
            {
                if (Database.Roulettes.RoulettesPoll.TryGetValue(WatchRoulette, out Table))
                {
                    Table.RemoveWatch(this.Player.UID);
                }
            }
        }
        #endregion

        #region AllClass
        public ServerFlag ClientFlag = ServerFlag.None;

        public Cryptography.TQCast5 Crypto;

        public Cryptography.DHKeyExchange.ServerKeyExchange DHKeyExchance;

        public ServerSockets.SecuritySocket Socket;

        // public Game.MsgServer.Lottery Lottery;

        public Role.Instance.DemonExterminator DemonExterminator;

        public Database.AchievementCollection Achievement;

        public Role.Instance.Wardrobe MyWardrobe;

        public Game.MsgServer.MsgStatus Status = new MsgStatus();

        public Role.Instance.Warehouse Warehouse;

        public Role.Instance.Equip Equipment;

        public Role.Instance.Inventory Inventory;

        public Role.Instance.Proficiency MyProfs;

        public System.SafeDictionary<uint, MsgGameItem> RelicFuse = new System.SafeDictionary<uint, MsgGameItem>();

        public Game.MsgNpc.Npc OnRemoveNpc;

        private static string Class(int Class)
        {
            if (Class >= 1005 && Class <= 1058) { return "Trojan"; }
            if (Class >= 2000 && Class <= 2058) { return "Warrior"; }
            if (Class >= 3000 && Class <= 3058) { return "DuneWanderer"; }
            if (Class >= 4000 && Class <= 4058) { return "Archer"; }
            if (Class >= 5000 && Class <= 5058) { return "Ninja"; }
            if (Class >= 5000 && Class <= 6058) { return "Monk"; }
            if (Class >= 6000 && Class <= 7058) { return "Pirate"; }
            if (Class >= 8000 && Class <= 8058) { return "Dragon-Warrior"; }
            if (Class >= 16000 && Class <= 16058) { return "Wind-Walker"; }
            if (Class >= 13000 && Class <= 13058) { return "WaterTaoist"; }
            if (Class >= 14000 && Class <= 14058) { return "FireTaoist"; }
            if (Class >= 9000 && Class <= 9058) { return "ThunderStriker"; }
            if (Class == 0) { return "Not selected"; }
            return "Not selected";

        }

        public Game.MsgServer.MsgProficiency prof;

        public Role.Instance.HeroRewards HeroRewards;

        public Role.Instance.SlotMachine SlotMachine = null;

        public Role.Instance.Vendor MyVendor;

        public Game.Booth MyBooth;
        public Booths MyBooths;
        public Role.Instance.House MyHouse;

        public Role.Instance.Trade MyTrade;

        public Role.Instance.Activeness Activeness;

        public Role.Player Player;

        public Role.GameMap Map = null;

        public Role.Instance.Team Team = null;

        public Role.Instance.Spell MySpells;

        public Role.Instance.Rune Rune;

        public Role.Instance.HairfaceStorage HairfaceStorage;

        public Role.Instance.Confiscator Confiscator;

        public Role.Instance.PerfectionEffect PerfectionStatus = new Role.Instance.PerfectionEffect();

        public Role.Instance.ExchangeShop MyExchangeShop;

        public Role.Instance.Beasts Beasts;

        public Role.Instance.HundredWeapons HundredWeapons;

        public Role.Instance.Mail MyMail;

        public Role.Instance.Ninja MyNinja;
       // public MsgTwistedFututr Twisted;

        internal uint HeavenVector;
        public Role.Instance.Archives MyArchives;

        public Role.Instance.Astredge MyAstredge;

        public Role.Instance.GuildSkill GuildSkill;

        public Role.Instance.DragonSkin DragonSkin;

        public System.SafeDictionary<uint, MsgGameItem> EonspiritSystem = new System.SafeDictionary<uint, MsgGameItem>();
        public Role.Instance.CollectionStorge Collection;
        #endregion

        #region GemsValues
        public uint RebornGem = 0;

        public ushort[] Gems = new ushort[14];

        public void AddGem(Role.Flags.Gem gem, ushort value)
        {
            if (gem == Role.Flags.Gem.NormalInfinityGem)
            {
                Status.MagicDamageDecrease += 1000;
                Status.PhysicalDamageDecrease += 1500;
                return;
            }
            if (gem == Role.Flags.Gem.RefinedInfinityGem)
            {
                Status.MagicDamageDecrease += 1500;
                Status.PhysicalDamageDecrease += 2000;
                return;
            }
            if (gem == Role.Flags.Gem.SuperInfinityGem)
            {
                Status.MagicDamageDecrease += 3000;
                Status.PhysicalDamageDecrease += 3000;
                return;
            }
            if (value == 15 || value == 10 || value == 5)
            {
                if (gem == Role.Flags.Gem.NormalDragonGem || gem == Role.Flags.Gem.RefinedDragonGem || gem == Role.Flags.Gem.SuperDragonGem)
                    Status.PhysicalPercent += value;
                else if (gem == Role.Flags.Gem.NormalPhoenixGem || gem == Role.Flags.Gem.RefinedPhoenixGem || gem == Role.Flags.Gem.SuperPhoenixGem)
                    Status.MagicPercent += value;
            }
            Gems[(byte)((byte)gem / 10)] += value;
        }

        public uint GemValues(Role.Flags.Gem gem)
        {
            return Gems[(byte)((byte)gem / 10)];
        }
        #endregion

        #region CalculatePower
        // [CombatAudit 2026-10-14] power of the last Shield / Stigma skill cast on this player ("effects of the same type do not stack, only the highest value is applied")
        public int ShieldPowerPercent = 120, StigmaPowerPercent = 30;
        public DateTime ShieldPowerUntil = DateTime.MinValue, StigmaPowerUntil = DateTime.MinValue;
        public void SetShieldPower(int percent, int seconds)
        {
            if (DateTime.Now < ShieldPowerUntil && ShieldPowerPercent > percent) return;
            ShieldPowerPercent = percent; ShieldPowerUntil = DateTime.Now.AddSeconds(seconds);
        }
        public void SetStigmaPower(int percent, int seconds)
        {
            if (DateTime.Now < StigmaPowerUntil && StigmaPowerPercent > percent) return;
            StigmaPowerPercent = percent; StigmaPowerUntil = DateTime.Now.AddSeconds(seconds);
        }
        public const int DefaultDefense2 = 10000;
        public uint AjustDefense
        {
            get
            {
                uint defence = (uint)(Status.Defence);
                uint nDefence = 0;
                if (Player.ContainFlag(Game.MsgServer.MsgUpdate.Flags.Shield) || Player.OnDefensePotion)
                {
                    // [CombatAudit 2026-10-14] Shield-type buffs use the casting skill's official power (XP Shield 30300 = x3, MagicShield 30110..30130 = +10..30%); other sources keep the old +20%
                    int shieldPct = (Player.ContainFlag(Game.MsgServer.MsgUpdate.Flags.Shield) && DateTime.Now < ShieldPowerUntil) ? ShieldPowerPercent : 120;
                    nDefence += (uint)Game.MsgServer.AttackHandler.Calculate.Base.MulDiv((int)defence, shieldPct, 100) - defence;
                }
                if (Player.ContainFlag(Game.MsgServer.MsgUpdate.Flags.DragonSwing))
                {
                    nDefence += (uint)Game.MsgServer.AttackHandler.Calculate.Base.MulDiv((int)defence, 115, 100) - defence;
                }
                if (Player.ContainFlag(MsgUpdate.Flags.DivineGuard))
                {
                    nDefence += (uint)Game.MsgServer.AttackHandler.Calculate.Base.MulDiv((int)defence, 300, 100) - defence;
                }
                return Player.ContainFlag(MsgUpdate.Flags.HookMoon) ? ((defence + nDefence) * Player.HookMoonAttackedPower / 100) : (defence + nDefence);
            }
        }

        public uint AjustAttack(uint Damage)
        {
            uint nAttack = 0;

            if (Player.ContainFlag(Game.MsgServer.MsgUpdate.Flags.Stigma) || Player.OnAttackPotion)
            {
                // [CombatAudit 2026-10-14] Stigma uses the casting skill's official power (30110..30130 = +10..30%); other sources keep the old +30%
                int stigmaPct = (Player.ContainFlag(Game.MsgServer.MsgUpdate.Flags.Stigma) && DateTime.Now < StigmaPowerUntil) ? StigmaPowerPercent : 30;
                nAttack += (uint)((Damage * (ulong)stigmaPct) / 100);
            }
            if (Status.PhysicalPercent > 0)
            {
                nAttack += (Damage * Status.PhysicalPercent) / 100;
            }
            if (Player.doFocus)
            {
                nAttack += (uint)Game.MsgServer.AttackHandler.Calculate.Base.MulDiv((int)Damage, Player.IntensifyDamage, 100) - Damage;
            }
            return Damage + nAttack;
        }
        public int GetDefense2()
        {

            return Player.Reborn >= 2 ? 5000 : DefaultDefense2;
        }

        public uint AjustCriticalStrike()
        {
            Role.Instance.RoleStatus Power;
            if (ExtraStatus.TryGetValue(Role.Instance.RoleStatus.StatuTyp.IncreasePStrike, out Power))
            {
                if (Power)
                    return Status.CriticalStrike + Power;
            }
            return Status.CriticalStrike;
        }

        public uint AjustMCriticalStrike()
        {
            Role.Instance.RoleStatus Power;
            if (ExtraStatus.TryGetValue(Role.Instance.RoleStatus.StatuTyp.IncreaseMStrike, out Power))
            {
                if (Power)
                    return Status.SkillCStrike + Power;
            }
            return Status.SkillCStrike;
        }

        public uint AjustImunity()
        {
            Role.Instance.RoleStatus Power;
            if (ExtraStatus.TryGetValue(Role.Instance.RoleStatus.StatuTyp.IncreaseImunity, out Power))
            {
                if (Power)
                    return Status.Immunity + Power;
            }
            return Status.Immunity;
        }

        public uint AjustBreakthrough()
        {
            Role.Instance.RoleStatus Power;
            if (ExtraStatus.TryGetValue(Role.Instance.RoleStatus.StatuTyp.IncreaseBreack, out Power))
            {
                if (Power)
                    return Status.Breakthrough + Power;


            }
            return Status.Breakthrough;
        }

        public uint AjustAntiBreack()
        {
            Role.Instance.RoleStatus Power;
            if (ExtraStatus.TryGetValue(Role.Instance.RoleStatus.StatuTyp.IncreaseAntiBreack, out Power))
            {
                if (Power)
                    return Status.Counteraction + Power;
            }
            return Status.Counteraction;
        }

        public uint AjustMagicDamageIncrease()
        {
            Role.Instance.RoleStatus Power;
            if (ExtraStatus.TryGetValue(Role.Instance.RoleStatus.StatuTyp.IncreaseFinalMAttack, out Power))
            {
                if (Power)
                    return Status.MagicDamageIncrease + Power;
            }
            return Status.MagicDamageIncrease;
        }

        public uint AjustMagicDamageDecrease()
        {
            Role.Instance.RoleStatus Power;
            if (ExtraStatus.TryGetValue(Role.Instance.RoleStatus.StatuTyp.IncreaseFinalMDamage, out Power))
            {
                if (Power)
                    return Status.MagicDamageDecrease + Power;
            }
            return Status.MagicDamageDecrease;
        }

        public uint AjustPhysicalDamageIncrease()
        {
            Role.Instance.RoleStatus Power;
            if (ExtraStatus.TryGetValue(Role.Instance.RoleStatus.StatuTyp.IncreaseFinalPAttack, out Power))
            {
                if (Power)
                    return Status.PhysicalDamageIncrease + Power;
            }
            return Status.PhysicalDamageIncrease;
        }

        public uint AjustPhysicalDamageDecrease()
        {
            Role.Instance.RoleStatus Power;
            if (ExtraStatus.TryGetValue(Role.Instance.RoleStatus.StatuTyp.IncreaseFinalPDamage, out Power))
            {
                if (Power)
                    return Status.PhysicalDamageDecrease + Power;
            }
            return Status.PhysicalDamageDecrease;
        }

        public uint AjustMagicAttack()
        {
            Role.Instance.RoleStatus Power;
            if (ExtraStatus.TryGetValue(Role.Instance.RoleStatus.StatuTyp.IncreaseMAttack, out Power))
            {
                if (Power)
                    return Status.MagicAttack + Power;
            }
            return (uint)(Status.MagicAttack);
        }

        public uint AjustMaxHitpoints()
        {
            Role.Instance.RoleStatus Power;
            if (ExtraStatus.TryGetValue(Role.Instance.RoleStatus.StatuTyp.IncreaseMaxHp, out Power))
            {
                if (Power)
                    return Status.MaxHitpoints + Power;
            }
            return Status.MaxHitpoints;
        }

        public uint AjustMaxAttack(uint damage)
        {
            Role.Instance.RoleStatus Power;
            if (ExtraStatus.TryGetValue(Role.Instance.RoleStatus.StatuTyp.IncreasePAttack, out Power))
            {
                if (Power)
                    return damage + Power;
            }
            return (uint)(damage);
        }
        #endregion

        #region SendBuffer
        public bool OnInterServer;

        public GameClient(ServerSockets.SecuritySocket _socket, bool _OnInterServer = false)
        {

            OnInterServer = _OnInterServer;
            CoatColorRule = new Qahr.Mobile.Role.Instance.CoatColorRule(this);
            MyExchangeShop = new Role.Instance.ExchangeShop(this);
            MyWardrobe = new Role.Instance.Wardrobe(this);

            Activeness = new Role.Instance.Activeness(this);
            HeroRewards = new Role.Instance.HeroRewards(this);
            RelicFuse = new System.SafeDictionary<uint, MsgGameItem>();
            EonspiritSystem = new System.SafeDictionary<uint, MsgGameItem>();
            MyArchives = new Role.Instance.Archives(this);
            MyAstredge = new Role.Instance.Astredge(this);
            DragonSkin = new Role.Instance.DragonSkin(this);
            GuildSkill = new Role.Instance.GuildSkill(this);
            Collection = new Role.Instance.CollectionStorge(this);
            MyNinja = new Role.Instance.Ninja(this);
            ExtraStatus = new System.Collections.Concurrent.ConcurrentDictionary<Role.Instance.RoleStatus.StatuTyp, Role.Instance.RoleStatus>();
            ArenaStatistic = new Game.MsgTournaments.MsgArena.User();
            DemonExterminator = new Role.Instance.DemonExterminator();
            MythSoulBag = new System.Collections.Concurrent.ConcurrentDictionary<uint, MsgGameItem>();
            PrizeInfo = new System.SafeDictionary<uint, Game.MsgServer.PrizeInfo>();
            Confiscator = new Role.Instance.Confiscator();
            //EnemyInvade = new HeroGathering.User(this);
            MedalStorage = new Qahr.Mobile.MedalStorage(this);
            MainRelic = new MainRelic(this);
            RelicFuseNew = new System.SafeDictionary<uint, MsgGameItem>();
            ClientFlag |= ServerFlag.None;
            if (_socket != null)
            {
                Socket = _socket;
                if (OnInterServer == false)
                {
                    Socket.Client = this;
                    Socket.Game = this;

                    DHKey = new Cryptography.DiffieHellman(Cryptography.DHKeyExchange.KeyExchange.Str_P, Cryptography.DHKeyExchange.KeyExchange.Str_G);
                    Crypto = new Cryptography.TQCast5();
                    Crypto.GenerateKey(System.Text.ASCIIEncoding.ASCII.GetBytes(Program.ServerConfig.LogginKey));
                    Socket.SetCrypto(Crypto);
                }
            }
            Player = new Role.Player(this);
            if (OnInterServer == false)
            {
                DHKeyExchance = new Cryptography.DHKeyExchange.ServerKeyExchange();

                if (_socket != null)
                {
                    Send(DHKeyExchance.CreateServerKeyPacket(DHKey));
                }
            }
        }

        public unsafe void Send(byte[] buffer)
        {
            try
            {
                if (global::Qahr.Mobile.AdminApi.AdminApiServer.CaptureActive) global::Qahr.Mobile.AdminApi.AdminApiServer.CaptureOutgoing(this, buffer); // [GmStudio] runcommand reply capture
                if (Fake || Socket.Alive == false || buffer == null)
                    return;

                if (buffer.Length > 1032)
                {
                    // MyConsole.WriteLine("[WARNING]: Sending packet with a huge length. ID: " + BitConverter.ToUInt16(buffer, 2));
                    return;
                }
                ushort length = BitConverter.ToUInt16(buffer, 0);
                if (length == 0)
                {
                    ServerSockets.Packet.WriteUInt16((ushort)(buffer.Length - 8), 0, buffer);
                }
                ServerSockets.Packet.WriteString("TQServer", buffer.Length - 8, buffer);
                ServerSockets.Packet stream = new ServerSockets.Packet(buffer);
                Socket.Send(stream);
            }
            catch (Exception e)
            {
                MyConsole.WriteException(e);
            }
        }

        public unsafe void Send(ServerSockets.Packet msg)
        {
            try
            {
                if (global::Qahr.Mobile.AdminApi.AdminApiServer.CaptureActive) global::Qahr.Mobile.AdminApi.AdminApiServer.CaptureOutgoing(this, msg); // [GmStudio] runcommand reply capture
                if (Fake || Socket.Alive == false || msg == null)
                    return;
                if (msg.Size > 1032)
                {
                    byte[] b = new byte[msg.Size];
                    fixed (byte* ptr = b)
                    {
                        msg.memcpy(ptr, msg.Memory, msg.Size);
                    }
                    // MyConsole.WriteLine("[WARNING]: Sending packet with a huge length. ID: " + BitConverter.ToUInt16(b, 2) + "Size" + msg.Size.ToString() + "");
                    return;
                }
                Socket.Send(msg);

            }
            catch (Exception e)
            {
                ushort PacketID = msg.ReadUInt16();
                //MyConsole.WriteLine("[WARNING]: Sending packet with a huge length. ID: " + PacketID);
                MyConsole.WriteException(e);
            }
        }
        #endregion

        #region Chat
        public void SendWhisper(string Messaj, string from, string to)
        {
            using (var rec = new ServerSockets.RecycledPacket())
            {
                var stream = rec.GetStream();
                var X = new Game.MsgServer.MsgMessage(Messaj, to, from, MsgMessage.MsgColor.red, MsgMessage.ChatMode.Whisper);
                X.Mesh = 1531003;
                X.Color = 4294967295;
                X.MessageUID1 = 550;
                var x2 = X.GetArray(stream);
                Send(x2);
            }
        }
        public void SendSysMesage(string Messaj, Game.MsgServer.MsgMessage.ChatMode ChatType = Game.MsgServer.MsgMessage.ChatMode.TopLeftSystem, Game.MsgServer.MsgMessage.MsgColor color = Game.MsgServer.MsgMessage.MsgColor.red, bool SendScren = false, bool self = false)
        {
            using (var rec = new ServerSockets.RecycledPacket())
            {
                var stream = rec.GetStream();
                if (!self)
                {
                    if (SendScren)
                        Player.View.SendView(new Game.MsgServer.MsgMessage(Messaj, color, ChatType).GetArray(stream), true);
                    else

                        Send(new Game.MsgServer.MsgMessage(Messaj, color, ChatType).GetArray(stream));
                }
                else
                {
                    if (SendScren)
                        Player.View.SendView(new Game.MsgServer.MsgMessage(Messaj, Player.Name, color, ChatType).GetArray(stream), true);
                    else

                        Send(new Game.MsgServer.MsgMessage(Messaj, Player.Name, color, ChatType).GetArray(stream));
                }
            }
        }
        public void CreateDialog(ServerSockets.Packet stream, string Text, string OptionText)
        {
            Game.MsgNpc.Dialog dialog = new Game.MsgNpc.Dialog(this, stream);
            dialog.AddText(Text);
            if (OptionText != "")
                dialog.AddOption(OptionText, 255);
            dialog.FinalizeDialog();
        }
        public void CreateBoxDialog(string Text)
        {
            using (var rec = new ServerSockets.RecycledPacket())
            {
                var stream = rec.GetStream();

                Game.MsgNpc.Dialog dialog = new Game.MsgNpc.Dialog(this, stream);
                dialog.CreateMessageBox(Text).FinalizeDialog(true);
            }
        }
        #endregion

        #region ItemPlayer
        public bool TryGetItem(uint UID, out Game.MsgServer.MsgGameItem item)
        {
            if (Equipment.TryGetValue(UID, out item))
            {
                return true;

            }
            if (Inventory.TryGetItem(UID, out item))
            {
                Database.ItemType.DBItem DBItem;
                if (Pool.ItemsBase.TryGetValue(item.ITEM_ID, out DBItem))
                {
                    if (DBItem.Save_Time > 0 && item.TimeLeftInMinutes == 0)
                    {
                        SendSysMesage("Active Time Stone First");
                        return false;
                    }
                }
                return true;

            }

            item = null; return false;
        }




        public DateTime OnChangeActionTime = new DateTime();
        public bool OnChangeAction
        {
            get
            {
                return DateTime.Now < OnChangeActionTime.AddMilliseconds(60); // عدد الثواني الى بياخدها Sit مابين كل ضربة وضربة لما بتقعد وتفضل تضرب
            }
        }

        public IEnumerable<Game.MsgServer.MsgGameItem> GetAllMainItems()
        {
            foreach (var item in Inventory.ClientItems.Values)
                yield return item;
            foreach (var item in Equipment.ClientItems.Values)
                yield return item;

        }

        public IEnumerable<Game.MsgServer.MsgGameItem> AllMyItems()
        {
            foreach (var item in Inventory.ClientItems.Values)
                yield return item;
            foreach (var item in Equipment.ClientItems.Values)
                yield return item;
            foreach (var Wh in Warehouse.ClientItems.Values)
            {
                foreach (var item in Wh.Values)
                    yield return item;
            }
            foreach (var item in MyWardrobe.GetAllItems())
                yield return item;

            foreach (var item in MinorRelics.Values)
            {
                yield return item;
            }

            foreach (var item in Rune.Objects)
                yield return item;
            foreach (var item in MythSoulBag.Values)
                yield return item;
            foreach (var item in EonspiritSystem.Values)
                yield return item;
            // كان فيه لوب تاني على Equipment هنا (مكرر مع اللي فوق) → كل غرض ملبوس كان بيتحفظ مرتين في الملف.
            // التحميل بيتجاهل النسخة التانية (TryAdd بالـ UID)، فالشيل ده مبيغيّرش الحالة بعد التحميل.
        }
        public IEnumerable<Game.MsgServer.MsgGameItem> AllMyTimeItems()
        {

            foreach (var item in Inventory.ClientItems.Values)
                if (item.Activate == 1)
                {
                    yield return item;
                }
            foreach (var item in Equipment.ClientItems.Values)
                if (item.Activate == 1)
                {
                    yield return item;
                }
            foreach (var Wh in Warehouse.ClientItems.Values)
            {
                foreach (var item in Wh.Values)
                    if (item.Activate == 1)
                    {
                        yield return item;
                    }
            }


            foreach (var item in MyWardrobe.GetAllItems())
                if (item.Activate == 1)
                {
                    yield return item;
                }
            foreach (var item in Rune.Objects)
                if (item.Activate == 1)
                {
                    yield return item;
                }
        }

        public IEnumerable<Game.MsgServer.MsgGameItem> AllPerfectionItems()
        {
            foreach (var item in Inventory.ClientItems.Values)
                if ((item.PerfectionProgress > 0 || item.PerfectionLevel > 0) && item.OwnerUID != 0 && item.IsEquip)
                    yield return item;
            foreach (var item in Equipment.ClientItems.Values)
                if ((item.PerfectionProgress > 0 || item.PerfectionLevel > 0) && item.OwnerUID != 0 && item.IsEquip)
                    yield return item;
            foreach (var Wh in Warehouse.ClientItems.Values)
            {
                foreach (var item in Wh.Values)
                    if ((item.PerfectionProgress > 0 || item.PerfectionLevel > 0) && item.OwnerUID != 0 && item.IsEquip)
                        yield return item;
            }

        }


        #endregion



        #region Calculate HP&&Mana
        public ushort CalculateHitPoint()
        {
            ushort valor = 0;
            if (Player.Class >= 1005 && Player.Class <= 1057)
                valor += (ushort)(Player.Agility * 3.45 + Player.Spirit * 3.45 + Player.Strength * 3.45 + Player.Vitality * 27.6);
            else
            {
                switch (Player.Class)
                {
                    case 1001:
                        valor += (ushort)(Player.Agility * 3.15 + Player.Spirit * 3.15 + Player.Strength * 3.15 + Player.Vitality * 25.2);
                        break;
                    case 1002:
                        valor += (ushort)(Player.Agility * 3.24 + Player.Spirit * 3.24 + Player.Strength * 3.24 + Player.Vitality * 25.9);
                        break;
                    case 1003:
                        valor += (ushort)(Player.Agility * 3.30 + Player.Spirit * 3.30 + Player.Strength * 3.30 + Player.Vitality * 26.4);
                        break;
                    case 1004:
                        valor += (ushort)(Player.Agility * 3.36 + Player.Spirit * 3.36 + Player.Strength * 3.36 + Player.Vitality * 26.8);
                        break;
                    default:
                        valor += (ushort)(Player.Agility * 3 + Player.Spirit * 3 + Player.Strength * 3 + Player.Vitality * 24);
                        break;
                }
            }
            return valor;

        }
        public ushort CalculateMana()
        {
            ushort valor = 0;
            if ((Player.Class >= 13005 && Player.Class <= 13057) || (Player.Class >= 14005 && Player.Class <= 14057))
                valor += (ushort)(Player.Spirit * 30);
            else
            {
                switch (Player.Class)
                {
                    case 14002:
                    case 13002: valor += (ushort)(Player.Spirit * 15); break;
                    case 14003:
                    case 13003: valor += (ushort)(Player.Spirit * 20); break;
                    case 14004:
                    case 13004: valor += (ushort)(Player.Spirit * 25); break;
                    default: valor += (ushort)(Player.Spirit * 5); break;
                }
            }
            return valor;
        }
        #endregion

        #region Teleport
        public void Pullback()
        {
            Teleport(Player.X, Player.Y, Player.Map, Player.DynamicID);
        }
        public void PullbackMT()
        {
            Teleport(Player.X, Player.Y, Player.Map, Player.DynamicID, false);
        }
        public void TeleportCallBack()
        {
            Teleport(Player.PMapX, Player.PMapY, Player.PMap, Player.PDinamycID);
        }
        public DateTime Protect = new DateTime();
        internal void ProtectAttack(int StampMiliSecounds)
        {
            Protect = DateTime.Now.AddMilliseconds(StampMiliSecounds);
        }

        /// <summary>Teleport to a named place (Data\places.txt, e.g. Places.TwinCity).</summary>
        public void Teleport(Place place, uint DinamycID = 0, bool revive = true, bool CanTeleport = false)
        {
            Teleport(place.X, place.Y, place.Map, DinamycID, revive, CanTeleport);
        }

        public void Teleport(ushort x, ushort y, uint MapID, uint DinamycID = 0, bool revive = true, bool CanTeleport = false)
        {
            Player.EndTeleGuildWar = false;
            Player.CountMonster2 = 0;
            Player.Stage = false;
            bool Active = false;
            if (Player.Map == 1038)
                Active = true;
            if (Player.Owner.Fake && CanTeleport)
            {
                using (var rec = new ServerSockets.RecycledPacket())
                {
                    var stream = rec.GetStream();
                    var data = new ActionQuery();
                    data.ObjId = Player.Owner.Player.UID;
                    Player.Owner.Player.Away = 1;
                    Player.Owner.Send(stream.ActionCreate(data));
                    Player.Owner.Player.View.SendView(Player.Owner.Player.GetArray(stream, false), false);
                }
            }
            //if (Player.Owner.Fake && Player.Owner.MyBot.Type == BotType.EventBots)
            //{
            //    ushort XX;
            //    ushort YY;
            //    BotHandle.XY(Player.Name, out XX, out YY);
            //    x = XX;
            //    y = YY;
            //}
            Player.CountMonster = 0;
            if (Player.ContainFlag(MsgUpdate.Flags.SoulShackle))
                return;
            if (MapID == 10250 || MapID == 10137)
            {
                if (this.Player.Reborn != 2)
                {
                    SendSysMesage("STR_ID_tUndergroundPalace[NotUpLevelText]@@");
                    return;
                }

            }
            if (InQualifier() && MapID != 9855 && MapID != 2068)
            {
                EndQualifier();
                if (ElitePkMap)
                    ElitePkMap = false;
                if (Player.InTeamPk)
                    Player.InTeamPk = false;
            }
            if (Player.Map == Game.MsgTournaments.MsgCaptureTheFlag.MapID && MapID != Game.MsgTournaments.MsgCaptureTheFlag.MapID)
            {
                using (var rec = new ServerSockets.RecycledPacket())
                {
                    var stream = rec.GetStream();

                    stream.CaptureTheFlagUpdateCreate((MsgCaptureTheFlagUpdate.Mode)9, 0, 0);
                    stream.CaptureTheFlagUpdateFinalize();
                    Send(stream);
                    stream.CaptureTheFlagUpdateCreate((MsgCaptureTheFlagUpdate.Mode)10, 0, 0);
                    stream.CaptureTheFlagUpdateFinalize();
                    Send(stream);
                }
            }
            if (Player.ObjInteraction != null)
            {
                using (var rec = new ServerSockets.RecycledPacket())
                {
                    var stream = rec.GetStream();
                    if (Player.ObjInteraction != null)
                    {
                        Player.InteractionEffect.AtkType = (ushort)Game.MsgServer.MsgAttackPacket.AttackID.InteractionStopEffect;
                        InteractQuery action2 = InteractQuery.ShallowCopy(Player.InteractionEffect);
                        Player.View.SendView(stream.InteractionCreate(action2), true);
                        action2.OpponentUID = Player.UID;
                        action2.UID = Player.ObjInteraction.Player.UID;
                        Player.View.SendView(stream.InteractionCreate(action2), true);
                        Player.OnInteractionEffect = false;
                        Player.Action = Role.Flags.ConquerAction.None;
                        Player.ObjInteraction.Player.OnInteractionEffect = false;
                        Player.ObjInteraction.Player.Action = Role.Flags.ConquerAction.None;
                        Player.ObjInteraction.Player.ObjInteraction = null;
                        Player.ObjInteraction = null;
                    }
                }
            }
            if (Player.ContainFlag((MsgUpdate.Flags)438) || Player.ContainFlag(MsgUpdate.Flags.SoulShackle) || Player.Map == 10001 && !Player.CanOut)
                return;
            if (!Pool.Constants.RemoveRide.Contains(Player.Map))
            {
                if (Player.ContainFlag(MsgUpdate.Flags.Ride))
                    Player.RemoveFlag(MsgUpdate.Flags.Ride);
            }
            if (Player.Map == 6521 || Player.Map == 10741 || Player.Map == 3071 || Player.Map == 3030 || Player.Map == 10550 || Player.Map == 1138 || Player.Map == 1138 || Player.Map == 10133 || Player.Map == 10134 || Player.Map == 2071 || Player.Map == 22330 || Player.Map == 22331 || Player.Map == 22332 || Player.Map == 22333 || Player.Map == 22334 || Player.Map == 22335 || Game.MsgTournaments.MsgSchedules.CurrentTournament.InTournament(this))
            {
                if (Player.ContainFlag(MsgUpdate.Flags.Ride))
                    Player.RemoveFlag(MsgUpdate.Flags.Ride);
            }
            if (!ProjectManager)
            {
                if (Player.Map == 6001 && CanTeleport == false)
                    return;
            }
            if (Player.Map == 1038 && Player.Alive == false && CanTeleport == false)
                return;
            using (var rec = new ServerSockets.RecycledPacket())
            {
                var stream = rec.GetStream();

                if (this.Player.ListPets != null) NewPetGuild.Death(this, stream);
                if (Player.OnAutoHunt)
                {
                    if (Player.AutoHuntExp > 0)
                    {
                        IncreaseAutoExperience(stream, Player.AutoHuntExp);
                    }
                    //  Send(MsgAutoHunt.AutoHuntCreate(stream, 7, 0));
                    // Send(MsgAutoHunt.AutoHuntCreate(stream, 3, 0));
                    // Send(MsgAutoHunt.AutoHuntCreate(stream, 2, 0));
                    Player.AutoHuntExp = 0;
                    Player.OnAutoHunt = false;
                }
                if (ExtraStatus.Count > 0 && MapID != 10137)
                {
                    foreach (var effectx in ExtraStatus)
                    {
                        Player.SendUpdate(stream, (MsgUpdate.DataType)effectx.Key, 0, 150, 0, false);
                    }
                    ExtraStatus.Clear();
                }
                if (Player.SetLocationType != 5 && Player.SetLocationType != 1 && Player.SetLocationType != 2 && Player.SetLocationType != 22)
                {
                    if (!Player.OnMyOwnServer && MapID != 3935 && MapID != 10137 && MapID != 8709 && MapID != 10478 && MapID != 5339 && MapID != 1002 && !InQualifier() && !IsWatching())
                        return;
                }

                if (MapID == 1011)
                {

                    if (x == 375 && y == 48)
                    {
                        if (this.Player.QuestGUI.CheckQuest(1352, MsgQuestList.QuestListItem.QuestStatus.Accepted))
                            Player.QuestGUI.IncreaseQuestObjectives(stream, 1352, 1);
                    }
                }


                if (Pool.Constants.MapCounterHits.Contains(Player.Map) || Player.Map == 1038 || Player.Map == 1357
                   || Game.MsgTournaments.MsgSchedules.CurrentTournament.InTournament(this))
                {
                    if (MapID != Player.Map)
                    {
                        SendSysMesage("", MsgMessage.ChatMode.FirstRightCorner);




                    }
                }

                if (Socket != null)//!= null for facke accounts.
                {
                    if (Socket.Alive == false)
                        return;
                }


                if (IsWatching() && Player.Map == 9855 && Player.Map == 22330 && Player.Map == 22331 && Player.Map == 22332 && Player.Map == 22333 && Player.Map == 22334 && Player.Map == 22335)
                {
                    if (ArenaWatchingGroup != null)
                        ArenaWatchingGroup.DoLeaveWatching(this);

                    else if (ElitePkWatchingGroup != null)
                        ElitePkWatchingGroup.DoLeaveWatching(this);

                }
                if (IsVendor)
                    MyVendor.StopVending(stream);
                if (InTrade)
                    MyTrade.CloseTrade(new MsgTrade.Trade() { Action = 5 });

                if (MapID == 601 || MapID == 1039)
                {
                    if (Player.HeavenBlessing > 0)
                    {
                        Player.SendUpdate(stream, Game.MsgServer.MsgUpdate.OnlineTraining.InTraining, Game.MsgServer.MsgUpdate.DataType.OnlineTraining);
                    }
                }
                if (Player.Map == 601 || Player.Map == 1039)
                {
                    if (MapID != 601 && MapID != 1039)
                        Player.SendUpdate(stream, Game.MsgServer.MsgUpdate.OnlineTraining.Review, Game.MsgServer.MsgUpdate.DataType.OnlineTraining);
                }


                Database.ServerDatabase.LoginQueue.Enqueue("[Teleport] Name: " + Player.Name + " Current Location: (" + Player.Map + "," + Player.X + "," + Player.Y + ") | Teleport Location: (" + MapID + "," + x + "," + y + ")");

                if (!Role.GameMap.CheckMap(MapID))
                {

                    MapID = 1002;
                    x = 410;
                    y = 354;
                }
                Role.GameMap GameMap;
                Role.GameMap.EnterMap((int)MapID);
                if (Pool.ServerMaps.TryGetValue(MapID, out GameMap))
                {
                    OnAutoAttack = false;
                    Player.RemoveBuffersMovements(stream);

                    Player.View.Clear(stream);


                    if (GameMap.BaseID != 0 && MapID != 10030)
                    {
                        var Pharaoh = new ActionQuery()//Edit By Pharaoh
                        {
                            ObjId = Player.UID,
                            dwParam = GameMap.BaseID,
                            Timestamp = (uint)System.Time32.timeGetTime().GetHashCode(),
                            NpcID = GameMap.BaseID, // [ProtoFix 2026-10-14] ActionQuery.NpcID is uint now
                            Type = ActionType.Teleport,
                            Fascing = (ushort)Role.Core.GetAngle(Player.X, Player.Y, x, y),
                            PositionX = x,
                            PositionY = y,
                            MapID = GameMap.BaseID,
                        };
                        Send(stream.ActionCreate(Pharaoh));
                    }

                    else if (MapID != 10030)
                    {
                        var Pharaoh = new ActionQuery()//Edit By Pharaoh
                        {
                            ObjId = Player.UID,
                            dwParam = MapID,
                            Timestamp = (uint)System.Time32.timeGetTime().GetHashCode(),
                            NpcID = MapID, // [ProtoFix 2026-10-14] ActionQuery.NpcID is uint now
                            Type = ActionType.Teleport,
                            Fascing = (ushort)Role.Core.GetAngle(Player.X, Player.Y, x, y),
                            PositionX = x,
                            PositionY = y,
                            MapID = MapID,
                        };
                        Send(stream.ActionCreate(Pharaoh));
                    }
                    if (MapID == 10030)
                    {
                        var Pharaoh = new ActionQuery()//Edit By Pharaoh
                        {
                            ObjId = Player.UID,
                            dwParam = MapID,
                            Timestamp = (uint)System.Time32.timeGetTime().GetHashCode(),
                            NpcID = MapID, // [ProtoFix 2026-10-14] ActionQuery.NpcID is uint now
                            Type = ActionType.Teleport,
                            Fascing = (ushort)Role.Core.GetAngle(Player.X, Player.Y, x, y),
                            PositionX = x,
                            PositionY = y,
                            MapID = MapID,
                        };
                        Send(stream.ActionCreate(Pharaoh));
                    }
                    if (Player.Map != 9855 && MapID != 10030)
                    {
                        var aaaction = new ActionQuery()
                        {
                            ObjId = Player.UID,
                            Type = 157,
                            dwParam = 2,
                            PositionX = x,
                            PositionY = y,
                            dwParam3 = MapID,
                            MapID = MapID
                        };
                        Send(stream.ActionCreate(aaaction));
                    }

                    var action = new ActionQuery()
                    {
                        ObjId = Player.UID,
                        dwParam = MapID,
                        Timestamp = (uint)System.Time32.timeGetTime().GetHashCode(),
                        NpcID = MapID, // [ProtoFix 2026-10-14] ActionQuery.NpcID is uint now
                        Type = ActionType.Teleport,
                        Fascing = (ushort)Role.Core.GetAngle(Player.X, Player.Y, x, y),
                        PositionX = x,
                        PositionY = y,
                        MapID = MapID,
                    };
                    Send(stream.ActionCreate(action));

                    if (MapID == 1780 && GameMap.BaseID == 0)
                    {
                        action = new ActionQuery()
                        {
                            ObjId = Player.UID,
                            Type = ActionType.SetMapColor,
                            dwParam = 0x323232,
                            PositionX = x,
                            PositionY = y
                        };
                        Send(stream.ActionCreate(action));

                    }
                    else if (MapID == 3846)
                    {
                        action = new ActionQuery()
                        {
                            ObjId = Player.UID,
                            Type = ActionType.SetMapColor,
                            dwParam = 16755370,
                            PositionX = x,
                            PositionY = y
                        };
                        Send(stream.ActionCreate(action));

                    }
                    else if (MapID == 10088 || MapID == 44455 || MapID == 44456)
                    {
                        action = new ActionQuery()
                        {
                            ObjId = Player.UID,
                            Type = ActionType.SetMapColor,
                            dwParam = 14535867,
                            PositionX = x,
                            PositionY = y
                        };
                        Send(stream.ActionCreate(action));
                    }
                    else
                    {

                        if (GameMap.ID == 3830 || GameMap.ID == 3831 || GameMap.ID == 3832)
                        {
                            action = new ActionQuery()
                            {
                                ObjId = Player.UID,
                                Type = ActionType.SetMapColor,
                                dwParam = GameMap.MapColor,
                                PositionX = x,
                                PositionY = y

                            };
                            Send(stream.ActionCreate(action));
                        }
                        else
                        {
                            action = new ActionQuery()
                            {
                                ObjId = Player.UID,
                                Type = ActionType.SetMapColor,
                                dwParam = 0,
                                PositionX = x,
                                PositionY = y

                            };
                            Send(stream.ActionCreate(action));
                        }
                    }
                    if (MapID == 1002)
                    {
                        Player.DynamicID = 0;
                    }
                    if (MapID == Player.Map && Player.DynamicID == DinamycID)
                    {
                        Pool.ServerMaps[Player.Map].Denquer(this);

                        Player.X = x;
                        Player.Y = y;
                        Pool.ServerMaps[MapID].Enquer(this);
                    }
                    else
                    {
                        Player.PDinamycID = Player.DynamicID;
                        Player.PMapX = Player.X;
                        Player.PMapY = Player.Y;

                        Pool.ServerMaps[Player.Map].Denquer(this);

                        Player.DynamicID = DinamycID;
                        Player.X = x;
                        Player.Y = y;
                        Player.PMap = Player.Map;

                        Player.Map = MapID;


                        Pool.ServerMaps[MapID].Enquer(this);
                    }

                    if (Player.Map == 9855)
                    {
                        if (InTeamQualifier())
                            Send(stream.MapStatusCreate(Map.ID, Map.ID, 19568946643047));
                        else if (ElitePkWatchingGroup != null || ElitePkMatch != null)
                            Send(stream.MapStatusCreate(Map.ID, Map.ID, 18173880847630407));
                        else
                            Send(stream.MapStatusCreate(Map.ID, Map.ID, Map.TypeStatus));
                    }
                    else if (GameMap.BaseID != 0)
                        Send(stream.MapStatusCreate(Map.BaseID, Map.BaseID, Map.TypeStatus));
                    else
                    {
                        if (Player.Map == 3935 && Player.Map == 10137 && Player.Map == 3535)
                            Send(stream.MapStatusCreate(Map.ID, Map.ID, 846641133264903));
                        else
                            Send(stream.MapStatusCreate(Map.ID, Map.ID, Map.TypeStatus));
                    }
                    Phone.MobileProtocol.SendFunctionFlags(this, stream);// QahrMobile: menu buttons (guild, lineage, ...)
                    Player.View.Role(true);

                    if (!Player.Alive && revive && Player.Map != 1038 && Player.Map != 1138 && Player.Map != 10741 && Player.Map != 3030 && Player.Map != 1138 && Player.Map != 10134 && Player.Map != 10133 && Player.Map != 3071)
                    {
                        Player.Revive(stream);
                    }
                    if (Player.ObjInteraction != null)
                    {
                        if (Role.Core.IsBoy(Player.Body))
                        {
                            Player.ObjInteraction.Teleport(x, y, MapID, DinamycID);
                        }
                    }
                    if (Player.Map == 12667 || Player.Map == 12546 || Player.Map == 12602 || Player.Map == 12275 || Player.Map == 3030 || Player.Map == 10741 || Player.Map == 12546 || Player.Map == 12605 || Player.Map == 1508 || Player.Map == 1770 || Player.Map == 12602 || Player.Map == 10741 || Player.Map == 2071 || Player.Map == 10133 || Player.Map == 3071 || Player.Map == 6521 || Player.Map == 5342 || Player.Map == 3825 || Player.Map == 5342 || Player.Map == 9988 || Player.Map == 6891 || Player.Map == 5599 || Player.Map == 8709 || Player.Map == 5339 || Player.Map == 8521 || Player.Map == 1005 || Player.Map == 3053 || Player.Map == 1801 || Player.Map == 12546 || Player.Map == 10133 || Player.Map == 11331 || Player.Map == 10741 || Player.Map == 3030 || Player.Map == 12799 || Player.Map == 12800 || Player.Map == 12099 || Player.Map == 12816 || Game.MsgTournaments.MsgSchedules.CurrentTournament.InTournament(this))
                        if (Player.ContainFlag(MsgUpdate.Flags.Ride))
                            Player.RemoveFlag(MsgUpdate.Flags.Ride);

                    if ((Player.Map == 12667 || Player.Map == 12275 || Player.Map == 12602 || Player.Map == 12605 || Player.Map == 3030 || Player.Map == 12546 || Player.Map == 12546 || Player.Map == 10741 || Player.Map == 12602 || Player.Map == 10741 || Player.Map == 2071 || Player.Map == 3030 || Player.Map == 10133 || Player.Map == 3071 || Player.Map == 6521 || Player.Map == 5342 || Player.Map == 3825 || Player.Map == 5342 || Player.Map == 9988 || Player.Map == 6891 || Player.Map == 5599 || Player.Map == 8709 || Player.Map == 5339 || Player.Map == 8521 || Player.Map == 1005 || Player.Map == 3053 || Player.Map == 1801 || Player.Map == 1508 || Player.Map == 1770 || Player.Map == 12546 || Player.Map == 10133 || Player.Map == 11331 || Player.Map == 10741 || Player.Map == 3030 || Player.Map == 12799 || Player.Map == 12800 || Player.Map == 12099 || Player.Map == 12816))
                    {
                        if (Player.ContainFlag(MsgUpdate.Flags.FatalStrike))
                            Player.RemoveFlag(MsgUpdate.Flags.FatalStrike);
                    }
                    if (MapID == 6001)
                        Player.Owner.JailStamp = DateTime.Now.AddMinutes(1);
                    if (Active)
                        Equipment.QueryEquipment(Equipment.Alternante, false);
                    Player.SendString(stream, MsgStringPacket.StringID.Effect, true, new string[] { "moveback" });
                }

            }

            //if (Player.Map == 3517 && !Player.Name.Contains("[GM]"))
            //{
            //    Teleport(410, 354, 1002);

            //    Player.Owner.Socket.Disconnect();

            //    Database.SystemBannedAccount.AddBan(
            //        Player.Owner.Player.UID,
            //        Player.Name,
            //        24,
            //        "Your Account Has Been Banned For Using Hack"
            //    );

            //    MyConsole.WriteLine(
            //        "Player [ " + Player.Name + " ] Banned To Bug [Join Map 3517]",
            //        ConsoleColor.White
            //    );

            //    Database.ServerDatabase.LoginQueue.Enqueue(
            //        "[Bug_Join_Map_3517] [UID Player] >> ( " + Player.UID +
            //        " ) [Name Player] >> ( " + Player.Name +
            //        " ) This Player opened Hack and Join Map 3517 and received (Ban 1 Day) "
            //    );

            //}
        }
        public void Shift(ushort X, ushort Y, ServerSockets.Packet stream, bool SendData = true, bool SentDragon = false, ushort Spell = 0)
        {
            Player.Px = Player.X;
            Player.Py = Player.Y;

            if (SendData)
            {
                ActionQuery action = new ActionQuery()
                {
                    ObjId = Player.UID,
                    Type = ActionType.FlashStep,
                    PositionX = X,
                    PositionY = Y
                };
                Player.View.SendView(stream.ActionCreate(action), true);
                Map.View.MoveTo<Role.IMapObj>(Player, X, Y);
                Player.X = X;
                Player.Y = Y;
                Player.View.Role(false, stream);
            }
            else if (SentDragon && Spell > 0)
            {
                var action = new ActionQuery()
                {
                    ObjId = Player.UID,
                    dwParam = Spell,
                    Type = 486,
                    PositionX = X,
                    PositionY = Y,
                    Timestamp = (uint)System.Time32.timeGetTime().GetHashCode(),
                };
                Player.View.SendView(stream.ActionCreate(action), true);
                Map.View.MoveTo<Role.IMapObj>(Player, X, Y);
                Player.X = X;
                Player.Y = Y;
                Player.View.Role(false, stream);
            }
            else
            {
                Map.View.MoveTo<Role.IMapObj>(Player, X, Y);
                Player.X = X;
                Player.Y = Y;
                Player.View.Role(false, null);
            }
        }
        #endregion

        #region UpdateLevel

        public ulong GainExperience(double Experience, ushort targetlevel)
        {
            var deltaLevel = Player.Level - targetlevel;
            if (deltaLevel >= 3)
            {
                if (deltaLevel >= 3 && deltaLevel <= 5)
                    Experience *= .7;
                else if (deltaLevel > 5 && deltaLevel <= 10)
                    Experience *= .2;
                else if (deltaLevel > 10 && deltaLevel <= 20)
                    Experience *= .1;
                else if (deltaLevel > 20)
                    Experience *= .05;
            }
            else if (deltaLevel < -15)
                Experience *= 1.8;
            else if (deltaLevel < -8)
                Experience *= 1.5;
            else if (deltaLevel < -5)
                Experience *= 1.3;

            return (ulong)Experience;
        }

        public void IncreaseExperience(ServerSockets.Packet stream, double Experience, Role.Flags.ExperienceEffect effect = Role.Flags.ExperienceEffect.None, bool questReward = false)
        {
            // [QuestEngineFix 2026-10-14] questReward = a quest prize: the official amount, without the server rate (x2), the
            // double-EXP potion and the rainbow gem (those are for hunting EXP). Hunting EXP is unchanged.
            if (Player.Map == 3030 || Player.Map == 9090 || Player.Map == 8832 || Player.Map == 8833 || Player.Map == 8834 || Player.Map == 8835 || Player.Map == 8836 || Player.Map == 8837 || Player.Map == 8838 || Player.Map == 8839 || Player.Map == 10741 || Player.Map == 3071 || Player.Map == 10133) return;//EXP in Map By Mohy ElDin ...
            if (Player.CursedTimer > 2)
            {
                return;
            }
            if (Player.Level < 140)
            {
                byte itemLevel = 0;
                if (Rune.IsEquipped("SoulChant", ref itemLevel))
                {
                    itemLevel = (byte)(10 + (itemLevel * 10));
                    Experience -= (double)((double)Experience * (double)itemLevel / 100);
                    if (Experience == 0) return;
                }

                if (effect != Role.Flags.ExperienceEffect.None)
                {
                    Player.SendString(stream, Game.MsgServer.MsgStringPacket.StringID.Effect, true, new string[1] { effect.ToString() });

                }

                if (!questReward)
                {
                Experience *= Program.ServerConfig.UserExpRate;
                Experience += Experience * GemValues(Role.Flags.Gem.NormalRainbowGem) / 100;


                if (Player.DExpTime > 0)
                    Experience *= Player.RateExp;
                }

                if (Player.Map == 1039)
                    Experience /= 100;

                if (Player.OnAutoHunt)
                {
                    Player.AutoHuntExp += (ulong)Experience;
                    return;
                }
                Player.Experience += (ulong)Experience;               
                while (Player.Experience >= Pool.LevelInfo[Database.DBLevExp.Sort.User][(byte)Player.Level].Experience)
                {
                    Player.Experience -= Pool.LevelInfo[Database.DBLevExp.Sort.User][(byte)Player.Level].Experience;
                    ushort newlev = (ushort)(Player.Level + 1);
                    UpdateLevel(stream, newlev);
                    if (Player.Level >= 140)
                    {
                        Player.Experience = 0;
                        break;
                    }
                }
                UpdateRebornLastLevel(stream);

                Player.SendUpdate(stream, (long)Player.Experience, Game.MsgServer.MsgUpdate.DataType.Experience, false);

            }
        }
        public void IncreaseAutoExperience(ServerSockets.Packet stream, double Experience, Role.Flags.ExperienceEffect effect = Role.Flags.ExperienceEffect.None)
        {
            if (Player.Map == 3030 || Player.Map == 9090 || Player.Map == 8832 || Player.Map == 8833 || Player.Map == 8834 || Player.Map == 8835 || Player.Map == 8836 || Player.Map == 8837 || Player.Map == 8838 || Player.Map == 8839 || Player.Map == 10741 || Player.Map == 3071 || Player.Map == 10133) return;// المابات الى الوحوش مش هتدي اكسبي فيها Edit By Mohy ElDin ...
            Player.Experience += (ulong)Experience;
#if NewActionHelperPOP
            {
            }
#else
            while (Player.Experience >= Pool.LevelInfo[Database.DBLevExp.Sort.User][(byte)Player.Level].Experience)
            {
                Player.Experience -= Pool.LevelInfo[Database.DBLevExp.Sort.User][(byte)Player.Level].Experience;
                ushort newlev = (ushort)(Player.Level + 1);
                UpdateLevel(stream, newlev);
                if (Player.Level >= 140)
                {
                    Player.Experience = 0;
                    break;
                }
            }

#endif
            UpdateRebornLastLevel(stream);

            Player.SendUpdate(stream, (long)Player.Experience, Game.MsgServer.MsgUpdate.DataType.Experience, false);
        }

        public void UpdateRebornLastLevel(ServerSockets.Packet stream)
        {
            if (Player.Reborn > 0)
            {
                if (Player.Reincarnation)
                {
                    if (Player.Level >= 110 && Player.Level < Player.SecoundeRebornLevel)
                    {
                        UpdateLevel(stream, Player.SecoundeRebornLevel, true);
                    }
                }
                else
                {
                    if (Player.Reborn == 1)
                    {
                        if (Player.Level >= 130 && Player.Level < Player.FirstRebornLevel)
                        {
                            UpdateLevel(stream, Player.FirstRebornLevel, true);
                        }
                    }
                    else if (Player.Reborn == 2)
                    {
                        if (Player.Level >= 130 && Player.Level < Player.SecoundeRebornLevel)
                            UpdateLevel(stream, Player.SecoundeRebornLevel, true);
                    }
                }
            }
        }

        public string InfoLevelUpdate(double amount = 600)
        {
            ulong ReceiveExperience = GainExpBall(amount, false, Role.Flags.ExperienceEffect.None, true);
            ulong MyExperince = Player.Experience;
            byte MyLevel = (byte)Player.Level;
            MyExperince += ReceiveExperience;
            while (MyExperince >= Pool.LevelInfo[Database.DBLevExp.Sort.User][(byte)MyLevel].Experience)
            {
                MyExperince -= Pool.LevelInfo[Database.DBLevExp.Sort.User][(byte)MyLevel].Experience;
                MyLevel++;
            }
            float Percentaj = (float)(Pool.LevelInfo[Database.DBLevExp.Sort.User][(byte)MyLevel].Experience / MyExperince);
            return "" + MyLevel + " (" + Percentaj + "%)";
        }

        public ulong GainExpBall(double amount = 600, bool sendMsg = false, Role.Flags.ExperienceEffect effect = Role.Flags.ExperienceEffect.None, bool JustCalculate = false, bool mentorexp = true)
        {
            if (Player.Level >= 140)
                return 0;
            if (sendMsg)
            {

                SendSysMesage("You have gained experience worth " + (amount * 1.0) / 600 + " exp ball(s).", Game.MsgServer.MsgMessage.ChatMode.System);
            }
            if (effect != Role.Flags.ExperienceEffect.None)
            {
                using (var rec = new ServerSockets.RecycledPacket())
                {
                    var stream = rec.GetStream();
                    Player.SendString(stream, Game.MsgServer.MsgStringPacket.StringID.Effect, true, new string[1] { effect.ToString() });
                }
            }
            var LevelDBExp = Pool.LevelInfo[Database.DBLevExp.Sort.User][(byte)Player.Level];
            if (LevelDBExp == null)
                return 0;

            var ReceiveExp = (ulong)Player.Experience * (ulong)LevelDBExp.UpLevTime / (double)LevelDBExp.Experience;
            ReceiveExp += amount;

            byte IncreaseLevel = (byte)Player.Level;

            var times = LevelDBExp.UpLevTime;

            while (IncreaseLevel < 140)
            {
                if (ReceiveExp < times)
                    break;
                ReceiveExp -= times;
                IncreaseLevel++;

                LevelDBExp = Pool.LevelInfo[Database.DBLevExp.Sort.User][IncreaseLevel];
                if (LevelDBExp == null)
                    break;

                times = LevelDBExp.UpLevTime;
            }
            if (times < 1)
            {
                return 0;
            }
            if (!JustCalculate)
            {
                using (var rec = new ServerSockets.RecycledPacket())
                {
                    var stream = rec.GetStream();
                    UpdateLevel(stream, IncreaseLevel, false, mentorexp);
                }
            }
            ReceiveExp /= times;

            LevelDBExp = Pool.LevelInfo[Database.DBLevExp.Sort.User][(byte)Player.Level];
            if (LevelDBExp == null)
                return 0;

            ulong CalculateEXp = (ulong)(ReceiveExp * LevelDBExp.Experience);
            if (!JustCalculate)
            {
                Player.Experience = CalculateEXp;
                using (var rec = new ServerSockets.RecycledPacket())
                {
                    var stream = rec.GetStream();
                    Player.SendUpdate(stream, (ulong)Player.Experience, Game.MsgServer.MsgUpdate.DataType.Experience, false);
                }
                using (var rec = new ServerSockets.RecycledPacket())
                {
                    var stream = rec.GetStream();
                    UpdateRebornLastLevel(stream);
                }
            }
            return CalculateEXp;
        }

        public ulong CalcExpBall(double amount, out ushort nextlevel)
        {
            if (Player.Level >= 140)
            { nextlevel = 0; return 0; }

            var LevelDBExp = Pool.LevelInfo[Database.DBLevExp.Sort.User][(byte)Player.Level];
            if (LevelDBExp == null)
            {
                nextlevel = 0;
                return 0;
            }

            var ReceiveExp = (ulong)Player.Experience * (ulong)LevelDBExp.UpLevTime / (double)LevelDBExp.Experience;
            ReceiveExp += amount;

            byte IncreaseLevel = (byte)Player.Level;

            var times = LevelDBExp.UpLevTime;

            while (IncreaseLevel < 140)
            {
                if (ReceiveExp < times)
                    break;
                ReceiveExp -= times;
                IncreaseLevel++;

                LevelDBExp = Pool.LevelInfo[Database.DBLevExp.Sort.User][IncreaseLevel];
                if (LevelDBExp == null)
                    break;

                times = LevelDBExp.UpLevTime;
            }

            if (times < 1)
            {
                nextlevel = IncreaseLevel;
                return 0;
            }
            ReceiveExp /= times;

            LevelDBExp = Pool.LevelInfo[Database.DBLevExp.Sort.User][(byte)Player.Level];
            if (LevelDBExp == null)
            { nextlevel = IncreaseLevel; return 0; }

            ulong CalculateEXp = (ulong)(ReceiveExp * LevelDBExp.Experience);

            nextlevel = IncreaseLevel;
            return CalculateEXp;
        }

        internal void LoseDeadExperience(Client.GameClient killer)
        {
            if (Fake)
                return;

            if (Player.Level >= 140)
                return;

            if (Player.ExpProtection > 0)
                return;

            var nextlevel = Pool.LevelInfo[Database.DBLevExp.Sort.User][(byte)(Player.Level)];
            if (nextlevel.Experience == 0)
            {
                return;
            }
            ulong loseexp = (ulong)((Player.Experience * (uint)(nextlevel.UpLevTime * nextlevel.MentorUpLevTime)) / nextlevel.Experience);
            double LoseExpPercent = (double)((double)loseexp / (double)nextlevel.Experience);

            if (Player.Experience > loseexp)
            {
                Player.Experience -= loseexp;
                using (var rec = new ServerSockets.RecycledPacket())
                {
                    var stream = rec.GetStream();
                    Player.SendUpdate(stream, (ulong)Player.Experience, Game.MsgServer.MsgUpdate.DataType.Experience);
                }
            }


            if (killer.Player.Level < Player.Level)
            {
                var killernextlevel = Pool.LevelInfo[Database.DBLevExp.Sort.User][(byte)(killer.Player.Level)];
                if (killernextlevel.Experience == 0)
                {
                    return;
                }
                double GetExp = (double)((double)100 / (double)killernextlevel.Experience) * (double)(loseexp * 100);
                killer.Player.Experience += (uint)GetExp;
                using (var rec = new ServerSockets.RecycledPacket())
                {
                    var stream = rec.GetStream();
                    killer.Player.SendUpdate(stream, (ulong)killer.Player.Experience, Game.MsgServer.MsgUpdate.DataType.Experience);
                }
            }
        }

        public void UpdateLevel(ServerSockets.Packet stream, ushort Level, bool REsetExp = false, bool mentorexp = true)
        {

            if (Level == Player.Level)
                return;
            if (Player.MyGuildMember != null)
            {
                Player.MyGuildMember.Level = Level;
            }
            if (REsetExp)
                Player.Experience = 0;
            uint OldLevel = Player.Level;
            Player.Level = Level;


            Player.SendUpdate(stream, Player.Level, Game.MsgServer.MsgUpdate.DataType.Level);
            ActionQuery action = new ActionQuery()
            {
                Type = ActionType.Leveled,
                ObjId = Player.UID,
                PositionX = Level
            };
            Player.View.SendView(stream.ActionCreate(action), true);


            if (Player.Reborn == 0 && (
                Database.AtributesStatus.IsWater(Player.Class)
                ? (Level < 111 || OldLevel < 110 && Level > 110)
                : (Level < 121 || OldLevel < 120 && Level > 120)))
            {
                Database.DataCore.AtributeStatus.GetStatus(Player);
                Player.SendUpdate(stream, Player.Strength, Game.MsgServer.MsgUpdate.DataType.Strength);
                Player.SendUpdate(stream, Player.Agility, Game.MsgServer.MsgUpdate.DataType.Agility);
                Player.SendUpdate(stream, Player.Spirit, Game.MsgServer.MsgUpdate.DataType.Spirit);
                Player.SendUpdate(stream, Player.Vitality, Game.MsgServer.MsgUpdate.DataType.Vitality);
            }
            else
            {
                if (OldLevel < Level)
                {
                    ushort artibute = (ushort)((Level - OldLevel) * 3);
                    Player.Atributes += artibute;
                    Player.SendUpdate(stream, Player.Atributes, Game.MsgServer.MsgUpdate.DataType.Atributes);
                }
            }

            if (Player.MyMentor != null && mentorexp)
            {
                var LevelUp = Pool.LevelInfo[Database.DBLevExp.Sort.User][(byte)OldLevel];
                Player.MyMentor.Mentor_ExpBalls += (uint)LevelUp.MentorUpLevTime;
                Role.Instance.Associate.Member mee;
                if (Player.MyMentor.Associat.ContainsKey(Role.Instance.Associate.Apprentice))
                {
                    if (Player.MyMentor.Associat[Role.Instance.Associate.Apprentice].TryGetValue(Player.UID, out mee))
                    {

                        mee.ExpBalls += (uint)LevelUp.MentorUpLevTime;
                    }
                }
            }
            Equipment.QueryEquipment(Equipment.Alternante, false);
            Player.HitPoints = (int)Status.MaxHitpoints;

            if (Player.Level <= 70 && Team != null)
            {
                var teamleader = Team.Leader;
                if (teamleader.Player.UID != Player.UID)
                {
                    if (Role.Core.GetDistance(teamleader.Player.X, teamleader.Player.Y, Player.X, Player.Y) < Role.RoleView.ViewThreshold)
                    {
                        if (teamleader.Player.Map != Player.Map)
                            return;
                        if (!teamleader.Player.Alive || teamleader.Player.Level < 70)
                            return;

                        teamleader.Player.VirtutePoints += (uint)(Player.Level * 10);
                        Team.SendTeam(new MsgMessage("Congratulations to leader, he have earned " + (Player.Level * 20).ToString() + " VirtuePoints by leveling up newbies!", MsgMessage.MsgColor.white, MsgMessage.ChatMode.Team).GetArray(stream), 0);

                    }
                }
            }
            UpdateRebornLastLevel(stream);
        }

        #endregion

        #region InfoAccount
        public Game.MsgServer.MsgLoginClient OnLogin = default(Game.MsgServer.MsgLoginClient);

        public unsafe Game.MsgServer.InteractQuery AutoAttack = default(Game.MsgServer.InteractQuery);



        public DateTime SaveMeleStamp = DateTime.Now.AddMilliseconds(ThreadPlayer.User_SaveMele);

        public GameClient SaveMele;
        public bool nSaveMele;
        public bool onSaveMele;

        public bool ProjectManager = false;

        public bool HelpDesk;

        // ===== GM authorization (security fix) =====
        // accounts.State arrives inside the encrypted login token (AccServer sends { EntityID, State }).
        // decrypted[0] = EntityID (OnLogin.Key), decrypted[1] = State (OnLogin.AccountHash).
        // Values: 255 = ProjectManager/Owner, 5 = GameHelper, 3 = GameMaster, 2 = Player.
        // This is the ONLY trustworthy source of privilege. The character name ([GM]) must never be
        // used to authorize destructive actions because it can be forged through name/INI injection.
        public byte AccountState
        {
            get { return (byte)OnLogin.AccountHash; }
        }

        /// <summary>Logged in from the mobile client (set by MobileProtocol.Login). Use Phone.MobileClient.Is(c) when c can be null.</summary>
        public bool IsMobile { get; set; }

        /// <summary>Real staff account (GameMaster / GameHelper / ProjectManager) by account State.</summary>
        public bool IsStaffAccount
        {
            get { return AccountState == 3 || AccountState == 5 || AccountState == 255; }
        }

        /// <summary>Highest tier (ProjectManager / Owner). Required for destructive, server-wide commands.</summary>
        public bool IsOwner
        {
            get { return AccountState == 255 || ProjectManager; }
        }

        // ===== NPC multi-step verification state (security fix) =====
        // The HelpDesk/Security NPCs implement "verify old secret" -> "write new secret" as two client
        // requests. The client fully controls the option number, so the write step must refuse to run
        // unless the matching verify step passed on THIS session. These flags enforce that.
        public bool PwdChangeVerified = false;
        public bool EmailChangeVerified = false;
        public bool PinChangeVerified = false;

        public uint ConnectionUID = 0;

        public int NpcCpsInput;

        public string MacAddress = "";

        public byte BanCount = 0;

        public ushort OnSoulSpell = 0;

        public int TerainMask = 0;

        public ulong ExpOblivion = 0;

        public byte TRyDisconnect = 2;

        public uint MoveNpcMesh;

        public uint MoveNpcUID;

        public uint UseItem = 0;

        public uint ActiveNpc = 0;
        public uint ActiveNpcBound = 0;
        public bool OnAutoAttack = false;
        public uint Vigor;
        public uint Online;
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

        public bool Fake = false;

        /// <summary>
        /// God DMG Mode - 20 Billion boost on all stats (Console: dmg PlayerName)
        /// </summary>
        public bool GodDmgMode = false;

        public bool IsGm
        {
            get
            {
                // Staff by the account State (accounts.State 3/5/255) or the PM / HelpDesk flags, never by the name.
                return Player != null && (IsStaffAccount || ProjectManager || HelpDesk);
            }
        }

        public DateTime CreateMatchesStamp;
        public DateTime JailStamp;

        public Database.AccountTable.AccountRegister AccountRegister = new Database.AccountTable.AccountRegister();

        public Database.AccountTable.ChangePassword ChangePassword = new Database.AccountTable.ChangePassword();

        internal static unsafe GameClient CharacterFromName(string p)
        {
            foreach (var kv in Pool.GamePoll) // [perf] live lock-free enumeration (no .Values snapshot)
            {
                var x = kv.Value;
                if (p == x.Player.Name)
                    return x;
            }
            return null;
        }

        public string AccountName(string name)
        {
            string username = "";
            using (var conn = new MySqlClientNs.MySqlConnection(DataHolder.ConnectionString))
            using (var cmd = new MySqlClientNs.MySqlCommand("select Username,Password from accounts where ID=@id", conn))
            {
                cmd.Parameters.AddWithValue("@id", Player.UID.ToString()); // [sqli] parameterized; same compare as ID='<uid>'
                conn.Open();
                using (var reader = cmd.ExecuteReader())
                {
                    if (reader.Read())
                    {
                        username = reader.GetString("Username");
                    }
                }
            }
            return username;
        }

        public string AccountName2(uint UID)
        {
            string usernameAndpasswrd = "";
            using (var conn = new MySqlClientNs.MySqlConnection(DataHolder.ConnectionString))
            using (var cmd = new MySqlClientNs.MySqlCommand("select Username,Password from accounts where ID=@id", conn))
            {
                cmd.Parameters.AddWithValue("@id", UID.ToString()); // [sqli] parameterized; same compare as ID='<uid>'
                conn.Open();
                using (var reader = cmd.ExecuteReader())
                {
                    if (reader.Read())
                    {
                        usernameAndpasswrd = reader.GetString("Username") + "|" + reader.GetString("Password");
                    }
                }
            }
            return usernameAndpasswrd;
        }
        public bool IsVendorr = false;
        public bool IsVendor
        {
            get
            {
                if (MyVendor != null)
                    return MyVendor.InVending;
                return false;
            }
        }

        public bool InTrade
        {
            get
            {
                if (MyTrade != null)
                    return MyTrade.WindowOpen;
                return false;
            }
        }
        #endregion

        #region EventRace
        public bool RaceGuard { get { return Player.ContainFlag(MsgUpdate.Flags.GodlyShield); } }
        public bool RaceDecelerated
        {
            get
            {
                return Player.ContainFlag(MsgUpdate.Flags.Deceleration);
            }
        }
        public bool RaceExcitement { get { return Player.ContainFlag(MsgUpdate.Flags.Accelerated); } }
        public int DirectionChange { get; internal set; }
        public bool RaceDizzy, RaceFrightened;
        public DateTime RaceExcitementStamp, GuardStamp, DizzyStamp, FrightenStamp, ExtraVigorStamp, DecelerateStamp;
        public uint RaceExcitementAmount, RaceExtraVigor;
        public void ApplyRacePotion(Game.MsgServer.MsgRacePotion.RaceItemType type, uint target)
        {
            switch (type)
            {
                case Game.MsgServer.MsgRacePotion.RaceItemType.FrozenTrap:
                    {
                        if (target != uint.MaxValue)
                        {
                            if (Map.IsFlagPresent(Player.X, Player.Y, Role.MapFlagType.Valid) == false)
                            {
                                Role.StaticRole role = new Role.StaticRole(Player.X, Player.Y);
                                role.DoFrozenTrap(Player.UID);
                                Map.AddStaticRole(role);

                                using (var rec = new ServerSockets.RecycledPacket())
                                {

                                    var stream = rec.GetStream();
                                    Player.View.SendView(stream, true);
                                }
                            }
                        }
                        else
                        {
                            Player.AddFlag(MsgUpdate.Flags.Freeze, 4, true);
                        }
                        break;
                    }
                case Game.MsgServer.MsgRacePotion.RaceItemType.RestorePotion:
                    {
                        Vigor += 2000;
                        if (Vigor > Status.MaxVigor)
                            Vigor = Status.MaxVigor;

                        using (var rec = new ServerSockets.RecycledPacket())
                        {
                            var stream = rec.GetStream();
                            Send(stream.ServerInfoCreate(Vigor));
                        }
                        break;
                    }
                case Game.MsgServer.MsgRacePotion.RaceItemType.ExcitementPotion:
                    {
                        if (RaceDecelerated)
                            Player.RemoveFlag(MsgUpdate.Flags.Deceleration);

                        Player.AddFlag(MsgUpdate.Flags.Accelerated, 15, true, 0, 50, 25);
                        RaceExcitementAmount = 50;
                        break;
                    }
                case Game.MsgServer.MsgRacePotion.RaceItemType.SuperExcitementPotion:
                    {
                        if (RaceDecelerated)
                            Player.RemoveFlag(MsgUpdate.Flags.Deceleration);

                        Player.AddFlag(MsgUpdate.Flags.Accelerated, 15, true, 0, 200, 100);
                        break;
                    }
                case Game.MsgServer.MsgRacePotion.RaceItemType.GuardPotion:
                    {

                        Player.AddFlag(MsgUpdate.Flags.GodlyShield, 10, true);
                        break;
                    }
                case Game.MsgServer.MsgRacePotion.RaceItemType.DizzyHammer:
                    {
                        Role.IMapObj obj;
                        if (Player.View.TryGetValue(target, out obj, Role.MapObjectType.Player))
                        {
                            var user = obj as Role.Player;
                            if (user != null)
                            {
                                if (!user.Owner.RaceGuard && !user.Owner.RaceFrightened)
                                {
                                    user.AddFlag(MsgUpdate.Flags.Dizzy, 5, true);
                                }
                            }
                        }

                        break;
                    }
                case Game.MsgServer.MsgRacePotion.RaceItemType.ScreamBomb:
                    {
                        using (var rec = new ServerSockets.RecycledPacket())
                        {

                            var stream = rec.GetStream();
                            MsgSpellAnimation MsgSpell = new MsgSpellAnimation(Player.UID, 0, Player.X, Player.Y, 9989, 0, 0);
                            MsgSpellAnimation.SpellObj AnimationObj = new MsgSpellAnimation.SpellObj(Player.UID, 0, MsgAttackPacket.AttackEffect.None);
                            MsgSpell.SetStream(stream);
                            MsgSpell.Send(this);
                        }

                        foreach (var user in Player.View.Roles(Role.MapObjectType.Player))
                        {
                            if (Role.Core.GetDistance(Player.X, Player.Y, user.X, user.Y) < 10)
                            {
                                var obj = user as Role.Player;
                                if (!obj.Owner.RaceGuard && !obj.Owner.RaceDizzy)
                                {
                                    obj.AddFlag(MsgUpdate.Flags.Frightened, 20, false);
                                }
                            }
                        }
                        break;
                    }
                case Game.MsgServer.MsgRacePotion.RaceItemType.SpiritPotion:
                    {

                        break;
                    }
                case Game.MsgServer.MsgRacePotion.RaceItemType.ChaosBomb:
                    {
                        using (var rec = new ServerSockets.RecycledPacket())
                        {

                            var stream = rec.GetStream();
                            MsgSpellAnimation MsgSpell = new MsgSpellAnimation(Player.UID, 0, Player.X, Player.Y, 9989, 0, 0);
                            MsgSpellAnimation.SpellObj AnimationObj = new MsgSpellAnimation.SpellObj(Player.UID, 0, MsgAttackPacket.AttackEffect.None);
                            MsgSpell.SetStream(stream);
                            MsgSpell.Send(this);
                        }

                        foreach (var user in Player.View.Roles(Role.MapObjectType.Player))
                        {
                            if (Role.Core.GetDistance(Player.X, Player.Y, user.X, user.Y) < 10)
                            {
                                var obj = user as Role.Player;
                                if (!obj.Owner.RaceGuard)
                                {
                                    obj.RemoveFlag(MsgUpdate.Flags.Dizzy);
                                    obj.AddFlag(MsgUpdate.Flags.Confused, 15, false);
                                }
                            }
                        }
                        break;
                    }
                case Game.MsgServer.MsgRacePotion.RaceItemType.SluggishPotion:
                    {
                        using (var rec = new ServerSockets.RecycledPacket())
                        {

                            var stream = rec.GetStream();
                            MsgSpellAnimation MsgSpell = new MsgSpellAnimation(Player.UID, 0, Player.X, Player.Y, 9989, 0, 0);
                            MsgSpellAnimation.SpellObj AnimationObj = new MsgSpellAnimation.SpellObj(Player.UID, 0, MsgAttackPacket.AttackEffect.None);
                            MsgSpell.SetStream(stream);
                            MsgSpell.Send(this);
                        }

                        foreach (var user in Player.View.Roles(Role.MapObjectType.Player))
                        {
                            if (Role.Core.GetDistance(Player.X, Player.Y, user.X, user.Y) < 10)
                            {
                                var obj = user as Role.Player;
                                if (!obj.Owner.RaceGuard)
                                {
                                    if (obj.Owner.RaceExcitement)
                                        obj.RemoveFlag(MsgUpdate.Flags.Accelerated);

                                    obj.AddFlag(MsgUpdate.Flags.Deceleration, 10, true, 0, 50, 25);
                                }
                            }
                        }

                        break;
                    }
                case Game.MsgServer.MsgRacePotion.RaceItemType.TransformItem:
                    {
                        for (int i = 0; i < 5; i++)
                        {
                            if (Player.RacePotions[i] != null)
                            {
                                if (Player.RacePotions[i].Type != MsgRacePotion.RaceItemType.TransformItem)
                                {
                                    using (var rec = new ServerSockets.RecycledPacket())
                                    {

                                        var stream = rec.GetStream();
                                        Send(stream.CreateRecePotion(new MsgRacePotion.RacePotion() { Amount = 0, Location = i + 1, PotionType = Player.RacePotions[i].Type }));
                                    }
                                    Player.RacePotions[i] = null;
                                }
                            }
                        }

                        {
                            int i = 0;
                            if (Player.RacePotions[i] == null)
                            {
                                int val = (int)MsgRacePotion.RaceItemType.TransformItem;
                                while (val == (int)MsgRacePotion.RaceItemType.TransformItem)
                                    val = Pool.GetRandom.Next((int)MsgRacePotion.RaceItemType.ChaosBomb, (int)MsgRacePotion.RaceItemType.SuperExcitementPotion);
                                Player.RacePotions[i] = new Game.MsgTournaments.MsgSteedRace.UsableRacePotion();
                                Player.RacePotions[i].Count = 1;
                                Player.RacePotions[i].Type = (MsgRacePotion.RaceItemType)val;

                                using (var rec = new ServerSockets.RecycledPacket())
                                {

                                    var stream = rec.GetStream();
                                    Send(stream.CreateRecePotion(new MsgRacePotion.RacePotion() { Amount = 1, Location = i + 1, PotionType = Player.RacePotions[i].Type }));
                                }
                            }
                        }
                        break;
                    }
            }
        }
        #endregion

        #region UpdateStamina
        public void UpdateStamina(ServerSockets.Packet stream, uint Stamina)
        {
            byte MaxStamina = (byte)(Player.HeavenBlessing > 0 ? 150 : 100);
            Player.Stamina = (ushort)Math.Min((int)(Player.Stamina + Stamina), MaxStamina);
            Player.SendUpdate(stream, Player.Stamina, MsgUpdate.DataType.Stamina);
        }
        #endregion
        public bool TryGetGlobalClientItem(uint UID, out Game.MsgServer.MsgGameItem item)
        {

            if (Equipment.TryGetValue(UID, out item))
                return true;
            if (Inventory.TryGetItem(UID, out item))
                return true;
            if (Warehouse.TryGetItem(UID, out item))
                return true;
            item = null; return false;
        }

        public DateTime CantAttack { get; set; }

        public bool ElitePkMap { get; set; }

        public DateTime ImpregnabilityEndTime { get; set; }

        public uint ImpregnabilityValue { get; set; }

        public System.SafeDictionary<uint, MsgGameItem> RelicFuseNew { get; set; }
    }
}