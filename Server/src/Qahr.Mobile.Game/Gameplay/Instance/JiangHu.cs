using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Qahr.Mobile.Game.MsgServer;

namespace Qahr.Mobile.Role.Instance
{
    public class JiangHu : IEnumerable<JiangHu.Stage>
    {


        public enum AttackFlag : uint
        {
            None = 0,
            NotHitFriends = 1 << 0,
            NotHitClanMembers = 1 << 1,
            NotHitGuildMembers = 1 << 2,
            NotHitAlliedGuild = 1 << 3,
            NoHitAlliesClan = 1 << 4
        }
        public static System.Collections.Concurrent.ConcurrentDictionary<uint, JiangHu> Poll = new System.Collections.Concurrent.ConcurrentDictionary<uint, JiangHu>();
        public static class JiangHuRanking
        {


            private static System.Collections.Concurrent.ConcurrentDictionary<uint, JiangHu> TopRank = new System.Collections.Concurrent.ConcurrentDictionary<uint, JiangHu>();
            public static JiangHu[] TopRank100 = null;
            private static object SyncRoot = new object();
            public static void UpdateRank(JiangHu jiang)
            {
                lock (SyncRoot)
                {
                    if (!TopRank.ContainsKey(jiang.UID))
                        TopRank.TryAdd(jiang.UID, jiang);
                    CalculateRanks();
                }
            }
            private static void CalculateRanks()
            {
                foreach (var jiang in TopRank.Values)
                    jiang.Rank = 0;
                var rankdictionar = TopRank.Values.ToArray();
                var ordonateRank = from jiang in rankdictionar orderby jiang.Inner_Strength descending select jiang;
                List<JiangHu> BackUp = new List<JiangHu>();
                byte x = 1;
                foreach (var jiang in ordonateRank)
                {
                    if (x == 101)
                        break;
                    jiang.Rank = x;
                    BackUp.Add(jiang);
                    x++;
                }
                TopRank100 = BackUp.ToArray();
                TopRank = new System.Collections.Concurrent.ConcurrentDictionary<uint, JiangHu>();
                foreach (var jiang in BackUp)
                    TopRank.TryAdd(jiang.UID, jiang);
                BackUp = null;

            }
        }

        public class Stage
        {
            public class Star
            {
                private int Count;
                public AtributesType Typ;
                public byte Level;
                public ushort UID;
                public bool Activate = false;

                public Star(int noumber)
                {
                    Count = noumber;
                }
            }

            private int Count;
            public bool Activate = false;
            public Star[] ArrayStars;
            public Stage(int noumber)
            {
                Count = noumber;
                ArrayStars = new Star[9];
                for (int x = 0; x < ArrayStars.Length; x++)
                {
                    ArrayStars[x] = new Star(x);
                }
            }
            [Flags]
            public enum AtributesType
            {
                None = 0,
                MaxLife,//1
                PAttack,//2
                MAttack,//3
                PDefense,//4
                Mdefense,//5
                FinalAttack,//6
                FinalMagicAttack,//7
                FinalDefense,//8
                FinalMagicDefense,//9
                CriticalStrike,//10
                SkillCriticalStrike,//11
                Immunity,//12
                Breakthrough,//13
                Counteraction,//14
                MaxMana//15
            }
        }

        internal ushort ValueToRoll(Stage.AtributesType status, byte level)
        {
            return (ushort)((ushort)status + level * 256);
        }
        internal byte GetValueLevel(ushort val)
        {
            return (byte)((val - (ushort)(val % 256)) / 256);
        }
        internal Stage.AtributesType GetValueType(uint val)
        {
            return (Stage.AtributesType)(val % 256);
        }
        internal byte CreateStarLevel(byte tryingultra)
        {
            byte value = 0;
            if (MyMath.Success(100))
                value = 1;
            if (MyMath.Success(80))
                value = 2;
            if (MyMath.Success(70))
                value = 3;
            if (MyMath.Success(50))
                value = 4;
            if (MyMath.Success(15))
                value = 5;
            if (MyMath.Success(1))
                value = 6;
            return value;
        }


        internal byte GetGrade()
        {
            int Grade = ArrayStages.Where(p => p.Activate == true).Count();
            if (Grade >= 9)
            {
                if (ArrayStages.Where(p => p.ArrayStars.Where(c => c.Level >= 1).Count() == 9).Count() == 9)
                    Grade++;
                if (ArrayStages.Where(p => p.ArrayStars.Where(c => c.Level >= 2).Count() == 9).Count() == 9)
                    Grade++;
                if (ArrayStages.Where(p => p.ArrayStars.Where(c => c.Level >= 3).Count() == 9).Count() == 9)
                    Grade++;
                if (ArrayStages.Where(p => p.ArrayStars.Where(c => c.Level >= 4).Count() == 9).Count() == 9)
                    Grade++;
                if (ArrayStages.Where(p => p.ArrayStars.Where(c => c.Level >= 5).Count() == 9).Count() == 9)
                    Grade++;
                if (ArrayStages.Where(p => p.ArrayStars.Where(c => c.Level >= 6).Count() == 9).Count() == 9)
                    Grade++;
            }
            return (byte)Grade;
        }

        private Random Rand = new Random();
        public Stage[] ArrayStages;
        public byte Talent = 3;
        public uint FreeCourse = 30000;
        public uint FreeTimeToday = 0;
        public uint UID;
        public DateTime StartCountDwon, CountDownEnd, CountDownMode, ThreadStamp = new DateTime();

        public string Name, CustomizedName;
        public uint RoundBuyPoints, Inner_Strength;
        public byte Level, Rank;
        public bool OnJiangMode = false;
        private bool CheckMode = false;
        public uint Time
        {
            get
            {
                return (uint)(CountDownEnd - new DateTime(1970, 1, 1).ToLocalTime()).TotalSeconds;
            }
        }
        public JiangHu(uint ID)
        {
            StartCountDwon = new DateTime();
            CountDownEnd = new DateTime();
            this.UID = ID;
            ArrayStages = new Stage[9];
            for (int x = 0; x < ArrayStages.Length; x++)
                ArrayStages[x] = new Stage(x);

            if (!Poll.ContainsKey(ID))
                Poll.TryAdd(ID, this);
            statusclient = new MsgStatus();
        }
        internal void CreateTime()
        {
            StartCountDwon = DateTime.Now;
            CountDownEnd = DateTime.Now.AddMinutes(Database.JianHuTable.GetMinutesOnTalent(Talent));
        }
        internal void ActiveJiangMode(Client.GameClient user)
        {
            CheckMode = false;
            OnJiangMode = true;
            user.Player.JiangHuActive = 1;
            user.Player.JiangHuTalent = Talent;
            SendInfo(user, Game.MsgServer.MsgJiangHuInfo.JiangMode.InfoStauts, true, user.Player.UID.ToString(), Talent.ToString(), OnJiangMode ? "1" : "2");
            using (var rec = new ServerSockets.RecycledPacket())
            {
                var stream = rec.GetStream();
                user.Player.View.ReSendView(stream);
            }
        }
        internal void DisableJiang(Client.GameClient user)
        {
            CheckMode = false;
            OnJiangMode = false;
            user.Player.JiangHuActive = 0;
            user.Player.JiangHuTalent = 0;
            SendInfo(user, Game.MsgServer.MsgJiangHuInfo.JiangMode.InfoStauts, true, user.Player.UID.ToString(), Talent.ToString(), OnJiangMode ? "1" : "2");
        }
        internal void LoginClient(ServerSockets.Packet stream, Client.GameClient user)
        {
            ThreadStamp = DateTime.Now;
            SendStatus(stream, user, user);
            SendInfo(user, Game.MsgServer.MsgJiangHuInfo.JiangMode.InfoStauts, true, user.Player.UID.ToString(), Talent.ToString(), OnJiangMode ? "1" : "2");
            if (OnJiangMode)
                ActiveJiangMode(user);

        }
        internal void CheckStatus(Client.GameClient user)
        {
            if (DateTime.Now > ThreadStamp.AddMinutes(1))
            {
                if (!CheckMode && user.Player.PkMode != Flags.PKMode.Jiang)
                {
                    CheckMode = true;
                    CountDownMode = DateTime.Now.AddMinutes(10);
                }
                if (OnJiangMode && user.Player.PkMode != Flags.PKMode.Jiang)
                {
                    if (DateTime.Now > CountDownMode)
                    {
                        OnJiangMode = false;
                        CheckMode = false;
                        user.Player.JiangHuActive = 0;
                        user.Player.JiangHuTalent = Talent;
                        SendInfo(user, Game.MsgServer.MsgJiangHuInfo.JiangMode.InfoStauts, true, user.Player.UID.ToString(), Talent.ToString(), OnJiangMode ? "1" : "2");
                    }
                }
                if (FreeCourse < 1000000)
                {

                    if (InTwinCastle(user.Player) && user.Player.PkMode == Flags.PKMode.Jiang)
                    {
                        StartCountDwon = StartCountDwon.AddMinutes(Database.JianHuTable.GetMinutesInCastle(Talent));
                        FreeCourse += Database.JianHuTable.GetFreeCourseInCastle(Talent);
                    }
                    else
                    {
                        FreeCourse += Database.JianHuTable.GetFreeCourse(Talent);
                        StartCountDwon = StartCountDwon.AddMinutes(1);
                    }
                    if (StartCountDwon > CountDownEnd)
                        GetReward(user);
                    SendInfo(user, Game.MsgServer.MsgJiangHuInfo.JiangMode.UpdateTime, false, FreeCourse.ToString(), Time.ToString());

                    if (FreeCourse % 10000 == 0)
                        GetReward(user);
                }
                else
                    FreeCourse = 1000000;

                ThreadStamp = DateTime.Now;
            }
        }
        internal void GetReward(Client.GameClient user)
        {
            do FreeCourse++; while (FreeCourse % 1000 != 0);
            SendInfo(user, Game.MsgServer.MsgJiangHuInfo.JiangMode.UpdateTime, false, FreeCourse.ToString(), Time.ToString());
            CreateTime();
        }
        internal void Kill(Client.GameClient user, Client.GameClient opponent)
        {
            if (opponent.Player.MyJiangHu != null)
            {
                uint opponentCourse = opponent.Player.MyJiangHu.FreeCourse % 10000;
                if (opponentCourse != 0)
                {
                    uint ReceiveCourse = opponentCourse * 30 / 100;
                    FreeCourse = (uint)Math.Min(1000000, ReceiveCourse);
                    SendInfo(user, Game.MsgServer.MsgJiangHuInfo.JiangMode.UpdateTime, false, FreeCourse.ToString(), Time.ToString());
                }
                if (opponent.Player.MyJiangHu.Talent > 1)
                {
                    opponent.Player.MyJiangHu.Talent -= 1;
                    Talent = (byte)Math.Min(5, Talent + 1);
                    user.Player.MyJiangHu.SendInfo(user, Game.MsgServer.MsgJiangHuInfo.JiangMode.UpdateTalent, true, user.Player.UID.ToString(), user.Player.MyJiangHu.Talent.ToString());
                }
            }
        }
        internal bool InTwinCastle(Role.Player location)
        {
            ushort x = location.X;
            ushort y = location.Y;
            if (location.Map != 1002)
                return false;

            if (x <= 357 && x >= 274 && y <= 277 && y >= 110)
                return true;
            if (x >= 335 && x <= 370 && y <= 235 && y >= 232)
                return true;
            if (x >= 281 && x <= 350 && y >= 268 && y <= 322)
                return true;
            if (x >= 247 && x <= 291 && y >= 200 && y <= 243)
                return true;

            return false;
        }
        public class BackUpStar
        {
            public byte Stage;
            public byte PositionStar;
            public Stage.Star Star;
        }
        public BackUpStar Buckupstar = null;
        internal void CreateRollValue(ServerSockets.Packet stream, Client.GameClient client, byte mStar, byte mStage, byte type = 0, byte HasKnowledgePill = 0)
        {
            Stage n_stage = ArrayStages[mStage - 1];
            if (mStage == 1)
                n_stage.Activate = true;
            if (!n_stage.Activate) return;
            Buckupstar = new BackUpStar();
            Buckupstar.PositionStar = mStar;
            Buckupstar.Stage = mStage;
            Buckupstar.Star = new Stage.Star(mStar - 1);
            Buckupstar.Star.Activate = true;
            if (HasKnowledgePill == 0)
                Buckupstar.Star.Level = CreateStarLevel((byte)(type + 1));
            else
                Buckupstar.Star.Level = (byte)Math.Min(6, new Random().Next(ArrayStages[mStage - 1].ArrayStars[mStar - 1].Level, 7));
            do
            {
                Buckupstar.Star.Typ = (Stage.AtributesType)Rand.Next(1, 16);
            }
            while (Database.JianHuTable.CultivateStatus[mStage].Contains((byte)Buckupstar.Star.Typ) == false);
            Buckupstar.Star.UID = ValueToRoll(Buckupstar.Star.Typ, Buckupstar.Star.Level);
            client.Send(stream.JiangHuUpdatesCreate(FreeCourse, MsgJiangHuUpdates.Action.CreateStar
                , mStar, mStage, Buckupstar.Star.UID, (byte)FreeTimeToday, RoundBuyPoints, type, HasKnowledgePill));
        }
        internal void CreateRollSuper(ServerSockets.Packet stream, Client.GameClient client, byte mStar, byte mStage, byte startype)
        {

            Stage n_stage = ArrayStages[mStage - 1];
            if (mStage == 1)
                n_stage.Activate = true;
            if (!n_stage.Activate) return;
            Buckupstar = new BackUpStar();
            Buckupstar.PositionStar = mStar;
            Buckupstar.Stage = mStage;

            Buckupstar.Star = new Stage.Star(mStar - 1);
            Buckupstar.Star.Activate = true;
            Buckupstar.Star.Level = 6;

            do
            {
                Buckupstar.Star.Typ = (Stage.AtributesType)startype;
            }
            while (Database.JianHuTable.CultivateStatus[mStage].Contains((byte)Buckupstar.Star.Typ) == false);

            Buckupstar.Star.UID = ValueToRoll(Buckupstar.Star.Typ, Buckupstar.Star.Level);

            client.Send(stream.JiangHuUpdatesCreate(FreeCourse, MsgJiangHuUpdates.Action.CreateStar
                           , mStar, mStage, Buckupstar.Star.UID, (byte)FreeTimeToday, RoundBuyPoints, startype, 0));


        }
        internal void ApplayNewStar(Client.GameClient client)
        {
            try
            {
                byte OldeGrade = GetGrade();

                if (Buckupstar == null)
                    return;
                Stage n_stage = ArrayStages[Buckupstar.Stage - 1];
                if (!n_stage.Activate) return;

                Stage.Star n_star = n_stage.ArrayStars[Buckupstar.PositionStar - 1];
                if (!n_star.Activate)
                {
                    n_star.Activate = true;
                }
                n_star.Level = Buckupstar.Star.Level;
                n_star.Typ = Buckupstar.Star.Typ;
                n_star.UID = Buckupstar.Star.UID;

                client.Equipment.QueryEquipment(client.Equipment.Alternante, false);

                if (Buckupstar != null)
                {

                    if (Buckupstar.Stage < 9)
                    {
                        if (Buckupstar.PositionStar == 9 && !ArrayStages[Buckupstar.Stage].Activate)
                        {
                            ArrayStages[Buckupstar.Stage].Activate = true;
                            byte nextStage = (byte)(ArrayStages.Where(p => p.Activate).Count() + 1);
                            SendInfo(client, Game.MsgServer.MsgJiangHuInfo.JiangMode.OpenStage, false, nextStage.ToString());
                        }
                    }
                }
                Buckupstar = null;
                byte NewGrade = GetGrade();
                if (NewGrade > OldeGrade)
                {
                    using (var rec = new ServerSockets.RecycledPacket())
                    {
                        var stream = rec.GetStream();
#if Arabic
  Program.SendGlobalPackets.Enqueue(new Game.MsgServer.MsgMessage("" + client.Player.Name + " increased Score Level to " + NewGrade.ToString() + "!", "ALLUSERS", "SYSTEM", Game.MsgServer.MsgMessage.MsgColor.red, Game.MsgServer.MsgMessage.ChatMode.JianHu).GetArray(stream));

#else
                        Server.SendGlobalPacket(new Game.MsgServer.MsgMessage("" + client.Player.Name + " increased Score Level to " + NewGrade.ToString() + "!", "ALLUSERS", "SYSTEM", Game.MsgServer.MsgMessage.MsgColor.red, Game.MsgServer.MsgMessage.ChatMode.JianHu).GetArray(stream));

#endif

                    }
                }
            }
            catch (Exception e) { MyConsole.WriteException(e); }
        }
        internal void CreateStatusAtributes(Client.GameClient client)
        {

#if Jiang
            if (!Program.ServerConfig.IsInterServer && client != null && client.OnInterServer == false)
            {
                statusclient = new MsgStatus();
                uint oldInner_Strength = Inner_Strength;
                Inner_Strength = 0;

                foreach (var nstage in ArrayStages)
                {

                    if (!nstage.Activate) continue;

                    var atr = nstage.ArrayStars.Where(p => p.UID != 0).ToArray();

                    byte count_double = 0;

                    Dictionary<uint, List<Stage.Star>> alignementstars = new Dictionary<uint, List<Stage.Star>>();
                    List<Stage.Star> normalstarts = new List<Stage.Star>();
                    ushort counts_alignements = 0;

                    for (byte x = 0; x < atr.Length; x++)
                    {
                        var atribut = atr[x];
                        count_double = 0;
                        bool AddFirstAtribute = false;
                        for (byte y = (byte)(x + 1); y < atr.Length; y++)
                        {
                            var atr2nd = atr[y];
                            if (atr2nd.Typ == atribut.Typ)
                            {
                                if (!AddFirstAtribute)
                                {
                                    if (!alignementstars.ContainsKey(counts_alignements))
                                    {
                                        alignementstars.Add(counts_alignements, new List<Stage.Star>());
                                        alignementstars[counts_alignements].Add(atribut);
                                    }
                                }
                                if (!alignementstars.ContainsKey(counts_alignements))
                                {
                                    alignementstars.Add(counts_alignements, new List<Stage.Star>());
                                    alignementstars[counts_alignements].Add(atr2nd);
                                }
                                else
                                    alignementstars[counts_alignements].Add(atr2nd);
                                AddFirstAtribute = true;
                                x = y;
                                count_double++;
                            }
                            else
                            {
                                break;
                            }
                        }
                        uint counts = 1;
                        if (count_double != 0)
                        {
                            counts = (byte)(count_double + 1);
                            counts_alignements++;
                        }
                        if (counts == 1)
                            normalstarts.Add(atribut);

                        count_double = 0;
                    }
                    foreach (var alignment in alignementstars.Values)
                    {
                        byte percent = (byte)Database.JianHuTable.AlignmentExtraPoints((byte)(alignment.Count - 1));
                        ushort Power = 0;
                        Stage.AtributesType AtrTyp = Stage.AtributesType.None;
                        foreach (var star in alignment)
                        {
                            Database.JianHuTable.Atribut Atri_bas = Database.JianHuTable.Atributes[star.UID];
                            Power += (ushort)(Atri_bas.Power + Math.Floor((double)(Atri_bas.Power * percent / 100)));
                            AtrTyp = star.Typ;
                        }
                        if (client != null)
                            ComputeStatus(client, AtrTyp, Power);

                        CalculateInner_StrengthAlignments(alignment);
                    }

                    foreach (var star in normalstarts)
                    {
                        Database.JianHuTable.Atribut Atri_bas = Database.JianHuTable.Atributes[star.UID];
                        if (client != null)
                            ComputeStatus(client, star.Typ, Atri_bas.Power);

                        Inner_Strength += Database.JianHuTable.GetStatusPoints(star.Level);

                    }
                }
                if (oldInner_Strength != Inner_Strength)
                    JiangHuRanking.UpdateRank(this);

            }
            if (client != null)
            {
                client.Status.Breakthrough += statusclient.Breakthrough;
                client.Status.Counteraction += statusclient.Counteraction;
                client.Status.CriticalStrike += statusclient.CriticalStrike;
                client.Status.PhysicalDamageIncrease += statusclient.PhysicalDamageIncrease;
                client.Status.PhysicalDamageDecrease += statusclient.PhysicalDamageDecrease;
                client.Status.MagicDamageDecrease += statusclient.MagicDamageDecrease;
                client.Status.MagicDamageIncrease += statusclient.MagicDamageIncrease;
                client.Status.Immunity += statusclient.Immunity;
                client.Status.MagicAttack += statusclient.MagicAttack;

                client.Status.MaxHitpoints += statusclient.MaxHitpoints;
                client.Status.MaxMana += statusclient.MaxMana;
                client.Status.MagicDefence += statusclient.MagicDefence;
                client.Status.MagicDefence += statusclient.MagicDefence;
                client.Status.MaxAttack += statusclient.MaxAttack;
                client.Status.MinAttack += statusclient.MinAttack;
                client.Status.Defence += statusclient.Defence;
                client.Status.SkillCStrike += statusclient.SkillCStrike;
            }

#endif
        }
        internal void CalculateInner_StrengthAlignments(List<Stage.Star> collection)
        {
            ushort points = 0;
            for (int x = 0; x < collection.Count; x++)
            {
                points += Database.JianHuTable.GetStatusPoints(collection[x].Level);
            }
            if (collection.Count > 0)
                Inner_Strength += (uint)(points + (points * (GetPercentALignment(collection.Count) * 10) / 100));
        }
        internal byte GetPercentALignment(int count)
        {
            if (count < 8)
                return (byte)(count - 1);
            if (count == 8)
                return (byte)count;
            else
                return (byte)(count + 1);
        }
        public Game.MsgServer.MsgStatus statusclient;
        internal void ComputeStatus(Client.GameClient client, Stage.AtributesType status, ushort Power)
        {

            switch (status)
            {
                case Stage.AtributesType.Breakthrough:
                    statusclient.Breakthrough += Power;
                    break;
                case Stage.AtributesType.Counteraction:
                    statusclient.Counteraction += Power; break;
                case Stage.AtributesType.CriticalStrike:
                    statusclient.CriticalStrike += Power; break;
                case Stage.AtributesType.FinalAttack:
                    statusclient.PhysicalDamageIncrease += Power; break;
                case Stage.AtributesType.FinalDefense:
                    statusclient.PhysicalDamageDecrease += Power; break;
                case Stage.AtributesType.FinalMagicAttack:
                    statusclient.MagicDamageIncrease += Power; break;
                case Stage.AtributesType.FinalMagicDefense:
                    statusclient.MagicDamageDecrease += Power; break;
                case Stage.AtributesType.Immunity:
                    statusclient.Immunity += Power; break;
                case Stage.AtributesType.MAttack:
                    {
                        statusclient.MagicAttack += Power; break;
                    }
                case Stage.AtributesType.MaxLife:
                    statusclient.MaxHitpoints += Power; break;
                case Stage.AtributesType.MaxMana:
                    statusclient.MaxMana += Power; break;
                case Stage.AtributesType.Mdefense:
                    {

                        statusclient.MagicDefence += Power; break;
                    }
                case Stage.AtributesType.PAttack:
                    {
                        statusclient.MaxAttack += Power;
                        statusclient.MinAttack += Power; break;
                    }
                case Stage.AtributesType.PDefense:
                    statusclient.Defence += Power; break;
                case Stage.AtributesType.SkillCriticalStrike:
                    statusclient.SkillCStrike += Power; break;
            }

        }
        internal unsafe void SendInfo(Client.GameClient client, Game.MsgServer.MsgJiangHuInfo.JiangMode Action = Game.MsgServer.MsgJiangHuInfo.JiangMode.CreateJiang, bool Screen = false, params string[] data)
        {
#if Jiang
            using (var rec = new ServerSockets.RecycledPacket())
            {
                var strean = rec.GetStream();

                // QahrMobile: CMsgOwnKongfuBase::Process modes 7 (status) and 14 (Kong Fu opened) take exactly two
                // strings, the uid and the Jiang Hu mode 1/2 (CheckGangHoodTag); the PC form puts the talent between them
                if ((Action == Game.MsgServer.MsgJiangHuInfo.JiangMode.InfoStauts || Action == Game.MsgServer.MsgJiangHuInfo.JiangMode.SetName)
                    && data != null && data.Length == 3)
                    data = new string[] { data[0], data[2] };
                strean.JiangHuInfoCreate(Action, data);
                if (Screen)
                    client.Player.View.SendView(strean, true);
                else
                    client.Send(strean);

            }
#endif
        }
        internal unsafe void SendStatus(ServerSockets.Packet stream, Client.GameClient client, Client.GameClient Attacked)
        {
            try
            {

                var dictionary = ArrayStages.Where(p => p.Activate);
                byte count = (byte)dictionary.Count<Stage>();


                uint atimer = 0;
                if (client.Player.UID != Attacked.Player.UID)
                    atimer = 13751297;
                else
                    atimer = 256000000;

                // QahrMobile: the mobile summary has 0 = my own Kong Fu / 1 = viewing another player's at the talent
                // byte (CMsgOwnKongfuImproveSummaryInfo::Process rejects anything >= 2)
                stream.JiangHuStatusCreate(CustomizedName, count, (byte)(client.Player.UID == Attacked.Player.UID ? 0 : 1), atimer, Attacked.Player.SubClass.StudyPoints, FreeTimeToday
                    , 0, RoundBuyPoints, dictionary.ToArray());
                client.Send(stream);


            }
            catch (Exception e) { MyConsole.WriteLine(e.ToString()); }
        }
        public IEnumerator<Stage> GetEnumerator()
        {
            foreach (var stage in ArrayStages)
                yield return stage;
        }
        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
        {
            return this.GetEnumerator();
        }
        public override string ToString()
        {
            if (StartCountDwon.Ticks > CountDownEnd.Ticks)
                CreateTime();
            uint SecoundesLeft = (uint)((CountDownEnd.Ticks - StartCountDwon.Ticks) / 10000000);
            if (Name != null)
            {
                if (Name.Contains('/'))
                    Name = Name.Replace("/", "");
            }
            if (CustomizedName != null)
            {
                if (CustomizedName.Contains('/'))
                    CustomizedName = CustomizedName.Replace("/", "");
            }
            Database.DBActions.WriteLine writer = new Database.DBActions.WriteLine('/');
            if (Name != null && CustomizedName != null)
            {
                writer.Add(UID).Add(Name).Add(CustomizedName).Add(Level).Add(Talent).Add(FreeTimeToday)
                    .Add((byte)(OnJiangMode ? 1 : 0)).Add(FreeCourse).Add(SecoundesLeft).Add(RoundBuyPoints);
            }
            foreach (var stage in ArrayStages)
            {
                writer.Add((byte)(stage.Activate ? 1 : 0));
                foreach (var star in stage.ArrayStars)
                    writer.Add((byte)(star.Activate ? 1 : 0)).Add(star.UID);
            }
            return writer.Close();
        }
    }
}
