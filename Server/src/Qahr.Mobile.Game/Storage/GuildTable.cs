using Qahr.Mobile.Client;
using Qahr.Mobile.Database.DBActions;
using Qahr.Mobile.Role.Instance;
using Qahr.Mobile.ServerSockets;
using Qahr.Mobile.WindowsAPI;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace Qahr.Mobile.Database
{
    public class GuildTable
    {
        //save ----------------------
        internal static void Save()
        {
            foreach (KeyValuePair<uint, Guild> keyValuePair in Guild.GuildPoll)
            {
                if (keyValuePair.Value.CanSave)
                {
                    Guild guild = keyValuePair.Value;
                    using (Write write = new Write("Guilds\\" + keyValuePair.Key.ToString() + ".txt"))
                    {
                        write.Add(guild.ToString()).Add(GuildTable.ToStringAlly(guild)).Add(GuildTable.ToStringEnemy(guild)).Add(guild.CTF_Exploits.ToString()).Add(guild.CTF_Next_ConquerPoints.ToString()).Add(guild.CTF_Next_Money.ToString()).Add(guild.CTF_Rank.ToString()).Add(guild.ClaimCtfReward.ToString()).Add(GuildTable.ToStringMessages(guild)).Add(GuildTable.ToStringConstruct(guild));
                        write.Execute(Database.DBActions.Mode.Open);
                        global::Qahr.Mobile.Database.Storage.SocialStorage.OnGuildSaved(keyValuePair.Key);
                    }
                }
            }
        }
        public static string ToStringAlly(Guild guild)
        {
            WriteLine writeLine = new WriteLine('/');
            writeLine.Add(guild.Ally.Count);
            foreach (Guild guild1 in (IEnumerable<Guild>)guild.Ally.Values)
                writeLine.Add(guild1.Info.GuildID);
            return writeLine.Close();
        }

        public static string ToStringEnemy(Guild guild)
        {
            WriteLine writeLine = new WriteLine('/');
            writeLine.Add(guild.Enemy.Count);
            foreach (Guild guild1 in (IEnumerable<Guild>)guild.Enemy.Values)
                writeLine.Add(guild1.Info.GuildID);
            return writeLine.Close();
        }
        //------------------------


        internal static void Load()
        {
            foreach (string fname in System.IO.Directory.GetFiles(Program.ServerConfig.DbLocation + "\\Guilds\\"))
            {
                using (DBActions.Read reader = new DBActions.Read(fname, true))
                {
                    if (reader.Reader())
                    {
                        //--------- guild info ------------------
                        DBActions.ReadLine GuildReader = new DBActions.ReadLine(reader.ReadString("0/"), '/');
                        uint ID = GuildReader.Read((uint)0);
                        if (ID > 100000)
                            continue;
                        if (ID > Role.Instance.Guild.Counter.Count)
                            Role.Instance.Guild.Counter.Set(ID);
                        Role.Instance.Guild guild = new Role.Instance.Guild(null, GuildReader.Read("None"), null);
                        guild.Info.GuildID = ID;
                        guild.Info.Level = GuildReader.Read((uint)0);
                        guild.Info.LeaderName = GuildReader.Read("None");
                        guild.Info.SilverFund = GuildReader.Read((long)0);
                        guild.Info.ConquerPointFund = GuildReader.Read((uint)0);
                        guild.Info.CreateTime = GuildReader.Read((uint)0);
                        guild.Bulletin = GuildReader.Read("None");
                        guild.BuletinEnrole = GuildReader.Read((int)0);
                        guild.UnionID = GuildReader.Read((uint)0);
                        guild.Info.Leaderid = GuildReader.Read((uint)0);
                        guild.Info.Material = GuildReader.Read((ulong)0);
                        guild.Info.Prestige = GuildReader.Read((ulong)0);
                        guild.Info.Recruitment_Flag = (Guild.ClassFlag)GuildReader.Read(0);
                        guild.Info.Recruitment_Battle_Power = GuildReader.Read((uint)0);
                        guild.Info.RecruitmentON = GuildReader.Read((byte)0) == (byte)1;
                        guild.Info.RecruitmentOFF = GuildReader.Read((byte)0) == (byte)1;
                        guild.Info.ArsenalBP = GuildReader.Read((uint)0);
                        guild.CommandID = GuildReader.Read((uint)0);
                        //----------------------------------


                        //----------------------------------------------------

                        //---------load ally ---------------------
                        LoadGuildAlly(ID, reader.ReadString("0/"));
                        //-----------------------------------

                        //---------load enemy --------------------
                        LoadGuildEnemy(ID, reader.ReadString("0/"));
                        //----------------------------------------

                        //---------load arsenals ------------------

                        //-----------------------------------------
                        try
                        {
                            guild.CTF_Exploits = reader.ReadUInt32(0);
                        }
                        catch
                        {
                            guild.CTF_Exploits = 0;
                        }
                        try
                        {
                            guild.CTF_Next_ConquerPoints = reader.ReadUInt32(0);
                            guild.CTF_Next_Money = reader.ReadUInt32(0);
                            guild.CTF_Rank = reader.ReadUInt32(0);
                            guild.ClaimCtfReward = reader.ReadUInt32(0);
                        }
                        catch
                        {

                        }
                        GuildTable.LoadMessages(guild, reader.ReadString("0/"));
                        GuildTable.LoadConstruct(guild, reader.ReadString("0/"));
                        if (!Role.Instance.Guild.GuildPoll.ContainsKey(guild.Info.GuildID))
                            Role.Instance.Guild.GuildPoll.TryAdd(guild.Info.GuildID, guild);



                        if (guild.UnionID != 0)
                        {
                            Role.Instance.Union union;
                            if (Role.Instance.Union.UnionPoll.TryGetValue(guild.UnionID, out union))
                            {
                                if (!union.Guilds.ContainsKey(guild.Info.GuildID))
                                    union.Guilds.TryAdd(guild.Info.GuildID, guild);
                            }
                        }
                    }
                }
            }
            // الجيلدات المنحلة في Guilds\Disbanded: العدّاد لازم يعدّي أعلى ID فيها عشان رقمها ميتعادش استخدامه
            uint maxDisbanded = GuildTable.MaxDisbandedID("Guilds");
            if (maxDisbanded <= 100000 && maxDisbanded > Role.Instance.Guild.Counter.Count)
                Role.Instance.Guild.Counter.Set(maxDisbanded);
            GuildTable.ExecuteAllyAndEnemy();
            GuildTable.LoadMembers();
            GuildTable.enemy.Clear();
            GuildTable.ally.Clear();

            // [perf] كان GC.Collect() هنا: GC كامل بيوقّف كل الثريدات (~21ms على heap 150MB). الـ runtime بيجمع لوحده.
        }
        //

        // نقل ملف جيلد/كلان منحل لـ <folder>\Disbanded عشان ميرجعش بعد الريستارت (Load بيقرا المجلد الأساسي بس)
        public static void MoveToDisbanded(string folder, uint id)
        {
            try
            {
                string baseDir = Program.ServerConfig.DbLocation + "\\" + folder + "\\";
                string src = baseDir + id.ToString() + ".txt";
                if (!File.Exists(src))
                    return;
                string dir = baseDir + "Disbanded\\";
                if (!Directory.Exists(dir))
                    Directory.CreateDirectory(dir);
                string dst = dir + id.ToString() + ".txt";
                if (File.Exists(dst))
                    dst = dir + id.ToString() + "_" + DateTime.Now.ToString("yyyyMMddHHmmss") + ".txt";
                File.Move(src, dst);
            }
            catch (Exception e) { MyConsole.WriteLine("MoveToDisbanded " + folder + " " + id + ": " + e.ToString()); }
        }

        // أعلى ID في <folder>\Disbanded (من اسم الملف: <id>.txt أو <id>_<timestamp>.txt)
        public static uint MaxDisbandedID(string folder)
        {
            uint max = 0;
            try
            {
                string dir = Program.ServerConfig.DbLocation + "\\" + folder + "\\Disbanded\\";
                if (!Directory.Exists(dir))
                    return 0;
                foreach (string fname in Directory.GetFiles(dir, "*.txt"))
                {
                    string name = Path.GetFileNameWithoutExtension(fname);
                    int cut = name.IndexOf('_');
                    if (cut >= 0)
                        name = name.Substring(0, cut);
                    uint id;
                    if (uint.TryParse(name, out id) && id > max)
                        max = id;
                }
            }
            catch (Exception e) { MyConsole.WriteLine("MaxDisbandedID " + folder + ": " + e.ToString()); }
            return max;
        }



        private static void LoadMembers()
        {
            IniFile iniFile = new IniFile("");
            foreach (string file in Directory.GetFiles(Program.ServerConfig.DbLocation + "\\Users\\"))
            {
                iniFile.FileName = file;
                uint UID = iniFile.ReadUInt32("Character", "UID", (uint)0);
                string str = iniFile.ReadString("Character", "Name", "None");
                uint key1 = iniFile.ReadUInt32("Character", "GuildID", (uint)0);
                if (key1 > 0)
                {
                    Guild guild;
                    if (Guild.GuildPoll.TryGetValue(key1, out guild))
                    {
                        ushort Body = iniFile.ReadUInt16("Character", "Body", (ushort)1002);
                        ushort Face = iniFile.ReadUInt16("Character", "Face", (ushort)0);
                        Guild.Member member = new Guild.Member();
                        member.UID = UID;
                        member.Mesh = (uint)Face * 10000 + (uint)Body;
                        member.Name = str;
                        member.Rank = (Role.Flags.GuildMemberRank)iniFile.ReadUInt32("Character", "GuildRank", 200);
                        member.Class = iniFile.ReadUInt32("Character", "Class", 0);
                        member.CpsDonate = iniFile.ReadUInt32("Character", "CpsDonate", 0);
                        member.MoneyDonate = iniFile.ReadInt64("Character", "MoneyDonate", 0);
                        member.PkDonation = iniFile.ReadUInt32("Character", "PkDonation", 0);
                        member.LastLogin = iniFile.ReadInt64("Character", "LastLogin", 0);
                        member.Level = (uint)iniFile.ReadUInt16("Character", "Level", (ushort)0);
                        member.PrestigePoints = (uint)iniFile.ReadUInt16("Character", "PrestigePoints", (ushort)0);
                        member.CTF_Exploits = iniFile.ReadUInt32("Character", "CTF_Exploits", 0);
                        member.RewardConquerPoints = iniFile.ReadUInt32("Character", "CTF_RCPS", 0);
                        member.RewardMoney = iniFile.ReadUInt32("Character", "CTF_RM", 0);
                        member.CTF_Claimed = iniFile.ReadByte("Character", "CTF_R", (byte)0);

                        ulong num3 = iniFile.ReadUInt64("Character", "DonationNobility", 0U);
                        Nobility user2;
                        if (Pool.NobilityRanking.TryGetValue(UID, out user2))
                            member.NobilityRank = (uint)user2.Rank;
                        else if (num3 >= 200000000)
                            member.NobilityRank = 5;
                        else if (num3 >= 100000000)
                            member.NobilityRank = 3;
                        else if (num3 >= 30000000)
                            member.NobilityRank = 1;
                        if ((int)guild.CommandID == (int)member.UID)
                            member.Command = 1U;
                        if (!guild.Members.ContainsKey(member.UID))
                            guild.Members.TryAdd(member.UID, member);
                        if (guild.UnionID > 0U)
                        {
                            Union getUnion = guild.GetUnion;
                            if (getUnion != null)
                            {
                                uint num4 = iniFile.ReadUInt32("Character", "UnionRank", 0);
                                member.UnionMem = Union.Member.CreateMember(member, (Union.Member.MilitaryRanks)num4);
                                member.UnionMem.Exploits = iniFile.ReadUInt32("Character", "UnionExploits", 0U);
                                member.UnionMem.MyTreasury = iniFile.ReadUInt32("Character", "Treasury", 0);
                                getUnion.UpdataDBRank(member.UnionMem, member.UnionMem.Rank);
                            }
                        }
                    }
                }
                else
                {
                    uint key2 = iniFile.ReadUInt32("Character", "UnionUID", 0);
                    Union union;
                    if (key2 > 0 && Union.UnionPoll.TryGetValue(key2, out union))
                    {
                        Union.Member member = new Union.Member();
                        ushort num5 = iniFile.ReadUInt16("Character", "Body", (ushort)1002);
                        ushort num6 = iniFile.ReadUInt16("Character", "Face", (ushort)0);
                        member.UID = UID;
                        member.Mesh = (uint)num6 * 10000 + (uint)num5;
                        member.Name = str;
                        member.Class = (uint)iniFile.ReadByte("Character", "Class", (byte)0);
                        member.Level = iniFile.ReadUInt16("Character", "Level", (ushort)0);
                        member.Rank = (Union.Member.MilitaryRanks)iniFile.ReadUInt32("Character", "UnionRank", 0U);
                        ulong num7 = iniFile.ReadUInt64("Character", "DonationNobility", 0);
                        Nobility user;
                        if (Pool.NobilityRanking.TryGetValue(UID, out user))
                            member.NobilityRank = user.Rank;
                        else if (num7 >= 200000000)
                            member.NobilityRank = Nobility.NobilityRank.Earl;
                        else if (num7 >= 100000000)
                            member.NobilityRank = Nobility.NobilityRank.Baron;
                        else if (num7 >= 30000000)
                            member.NobilityRank = Nobility.NobilityRank.Knight;
                        member.Exploits = iniFile.ReadUInt32("Character", "UnionExploits", 0);
                        member.MyTreasury = iniFile.ReadUInt32("Character", "Treasury", 0);
                        union.UpdataDBRank(member, member.Rank);
                        if (member.Rank == Union.Member.MilitaryRanks.Emperor && (int)union.EmperrorUID != (int)member.UID)
                            member.Rank = Union.Member.MilitaryRanks.Member;
                        if (!union.Members.ContainsKey(member.UID))
                            union.Members.TryAdd(member.UID, member);
                    }
                }
            }
        }
        public static void ExecuteAllyAndEnemy()
        {
            foreach (KeyValuePair<uint, List<uint>> keyValuePair in GuildTable.ally)
            {
                foreach (uint key in keyValuePair.Value)
                {
                    Guild guild;
                    if (Guild.GuildPoll.TryGetValue(key, out guild) && Guild.GuildPoll.ContainsKey(keyValuePair.Key))
                        Guild.GuildPoll[keyValuePair.Key].Ally.TryAdd(guild.Info.GuildID, guild);
                }
            }
            foreach (KeyValuePair<uint, List<uint>> keyValuePair in GuildTable.enemy)
            {
                foreach (uint key in keyValuePair.Value)
                {
                    Guild guild;
                    if (Guild.GuildPoll.TryGetValue(key, out guild) && Guild.GuildPoll.ContainsKey(keyValuePair.Key))
                        Guild.GuildPoll[keyValuePair.Key].Enemy.TryAdd(guild.Info.GuildID, guild);
                }
            }
        }

        public static Dictionary<uint, List<uint>> ally = new Dictionary<uint, List<uint>>();
        public static Dictionary<uint, List<uint>> enemy = new Dictionary<uint, List<uint>>();
        public static void LoadGuildAlly(uint id, string line)
        {
            ReadLine readLine = new ReadLine(line, '/');
            int num = readLine.Read(0);
            for (int index = 0; index < num; ++index)
            {
                if (GuildTable.ally.ContainsKey(id))
                    GuildTable.ally[id].Add(readLine.Read((uint)0));
                else
                    GuildTable.ally.Add(id, new List<uint>()
          {
            readLine.Read(0U)
          });
            }
        }

        public static void LoadGuildEnemy(uint id, string line)
        {
            ReadLine readLine = new ReadLine(line, '/');
            int num = readLine.Read(0);
            for (int index = 0; index < num; ++index)
            {
                if (GuildTable.enemy.ContainsKey(id))
                    GuildTable.enemy[id].Add(readLine.Read((uint)0));
                else
                    GuildTable.enemy.Add(id, new List<uint>()
          {
            readLine.Read((uint)0)
          });
            }
        }



        public static string ToStringMessages(Guild guild)
        {
            WriteLine writeLine = new WriteLine('/');
            writeLine.Add((uint)guild.Message.Count);
            foreach (Guild.GuildMessage guildMessage in guild.Message)
            {
                writeLine.Add(guildMessage.message);
                writeLine.Add(guildMessage.Time);
            }
            return writeLine.Close();
        }

        public static string ToStringConstruct(Guild guild)
        {
            WriteLine writeLine = new WriteLine('/');
            writeLine.Add(guild.Constructs.Count);
            foreach (Guild.Construct construct in (IEnumerable<Guild.Construct>)guild.Constructs.Values)
            {
                writeLine.Add(construct.ID);
                writeLine.Add(construct.Level);
                writeLine.Add(construct.Exp);
                writeLine.Add(construct.Beast);
                writeLine.Add(construct.v2);
                writeLine.Add(construct.v3);
                writeLine.Add(construct.v4);
            }
            return writeLine.Close();
        }

        public static void LoadMessages(Guild guild, string line)
        {
            ReadLine readLine = new ReadLine(line, '/');
            int num = readLine.Read(0);
            for (int index = 0; index < num; ++index)
                guild.Message.Add(new Guild.GuildMessage()
                {
                    message = readLine.Read(""),
                    Time = readLine.Read((uint)0)
                });
        }

        public static void LoadConstruct(Guild guild, string line)
        {
            guild.Constructs = new ConcurrentDictionary<uint, Guild.Construct>();
            ReadLine readLine = new ReadLine(line, '/');
            int num = readLine.Read(0);
            for (int index = 0; index < num; ++index)
            {
                uint id = readLine.Read((uint)0);
                uint level = readLine.Read((uint)0);
                ulong _Exp = readLine.Read((ulong)0);
                ulong beast = readLine.Read((ulong)0);
                ulong _v2 = readLine.Read((ulong)0);
                ulong _v3 = readLine.Read((ulong)0);
                ulong _v4 = readLine.Read((ulong)0);
                Guild.Construct construct = new Guild.Construct(id, level, beast, _Exp, _v2, _v3, _v4);
                guild.Constructs.Add<uint, Guild.Construct>((uint)((ulong)(id * 100) + beast), construct);
            }
        }
    }
}
