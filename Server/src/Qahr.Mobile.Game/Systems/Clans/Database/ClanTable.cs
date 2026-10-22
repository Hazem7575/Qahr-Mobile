using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Qahr.Mobile.Database
{
    public class ClanTable
    {

        internal static void Save()
        {
            foreach (var obj in Role.Instance.Clan.Clans)
            {

                var clan = obj.Value;
                using (DBActions.Write writer = new DBActions.Write("Clans\\" + obj.Key + ".txt"))
                {
                    writer.Add(clan.ToString())
                        .Add(clan.SaveAlly())
                        .Add(clan.SaveEnemy()).Execute(DBActions.Mode.Open);
                    global::Qahr.Mobile.Database.Storage.SocialStorage.OnClanSaved(obj.Key);
                }
            }
        }

        internal static void Load()
        {
            foreach (string fname in System.IO.Directory.GetFiles(Program.ServerConfig.DbLocation + "\\Clans\\"))
            {
                using (DBActions.Read reader = new DBActions.Read(fname, true))
                {
                    if (reader.Reader())
                    {
                        Role.Instance.Clan clan = new Role.Instance.Clan();
                        clan.Load(reader.ReadString(""));
                        if (clan.ID > Role.Instance.Clan.CounterClansID.Count)
                            Role.Instance.Clan.CounterClansID.Set(clan.ID + 1);
                        LoadclanAlly(clan.ID, reader.ReadString(""));
                        LoadclanEnemy(clan.ID, reader.ReadString(""));

                        if (!Role.Instance.Clan.Clans.ContainsKey(clan.ID))
                            Role.Instance.Clan.Clans.TryAdd(clan.ID, clan);
                    }
                }
            }
            // الكلانات المنحلة في Clans\Disbanded: العدّاد يعدّي أعلى ID فيها (نفس أسلوب ID + 1 اللي فوق)
            uint maxDisbanded = GuildTable.MaxDisbandedID("Clans");
            if (maxDisbanded > 0 && maxDisbanded >= Role.Instance.Clan.CounterClansID.Count)
                Role.Instance.Clan.CounterClansID.Set(maxDisbanded + 1);
            LoadMemebers();
            ClanExecuteAllyAndEnemy();
            // [perf] كان GC.Collect() هنا: GC كامل بيوقّف كل الثريدات (~21ms على heap 150MB). الـ runtime بيجمع لوحده.
        }
        public static void ClanExecuteAllyAndEnemy()
        {
            foreach (var obj in clanally)
            {
                foreach (var clan in obj.Value)
                {
                    Role.Instance.Clan alyclan;
                    if (Role.Instance.Clan.Clans.TryGetValue(clan, out alyclan))
                    {
                        if (Role.Instance.Clan.Clans.ContainsKey(obj.Key))
                        {
                            Role.Instance.Clan.Clans[obj.Key].Ally.TryAdd(alyclan.ID, alyclan);
                        }
                    }
                }
            }
            foreach (var obj in clanenemy)
            {
                foreach (var clan in obj.Value)
                {
                    Role.Instance.Clan enemyclan;
                    if (Role.Instance.Clan.Clans.TryGetValue(clan, out enemyclan))
                    {
                        if (Role.Instance.Clan.Clans.ContainsKey(obj.Key))
                        {
                            Role.Instance.Clan.Clans[obj.Key].Enemy.TryAdd(enemyclan.ID, enemyclan);
                        }
                    }
                }
            }
        }
        private static void LoadMemebers()
        {
            WindowsAPI.IniFile ini = new WindowsAPI.IniFile("");
            foreach (string fname in System.IO.Directory.GetFiles(Program.ServerConfig.DbLocation + "\\Users\\"))
            {
                ini.FileName = fname;

                uint UID = ini.ReadUInt32("Character", "UID", 0);
                string Name = ini.ReadString("Character", "Name", "None");
                uint ClanID = ini.ReadUInt32("Character", "ClanID", 0);
                if (ClanID != 0)
                {
                    Role.Instance.Clan Clan;
                    if (Role.Instance.Clan.Clans.TryGetValue(ClanID, out Clan))
                    {
                        Role.Instance.Clan.Member member = new Role.Instance.Clan.Member();
                        member.UID = UID;
                        member.Name = Name;
                        member.Rank = (Role.Instance.Clan.Ranks)ini.ReadUInt16("Character", "ClanRank", 200);
                        member.Class = ini.ReadUInt32("Character", "Class", 0);
                      
                        member.Level = (byte)ini.ReadUInt16("Character", "Level", 0);
                        member.Donation = ini.ReadUInt32("Character", "ClanDonation", 0);

                        if (!Clan.Members.ContainsKey(member.UID))
                            Clan.Members.TryAdd(member.UID, member);
                    }
                }
            }
        }
        public static Dictionary<uint, List<uint>> clanally = new Dictionary<uint, List<uint>>();
        public static Dictionary<uint, List<uint>> clanenemy = new Dictionary<uint, List<uint>>();
        private static void LoadclanAlly(uint id, string line)
        {
            Database.DBActions.ReadLine reader = new DBActions.ReadLine(line, '/');
            int count = reader.Read((int)0);
            for (int x = 0; x < count; x++)
            {
                uint obj = reader.Read((uint)0);
                if (clanally.ContainsKey(id))
                    clanally[id].Add(obj);
                else
                    clanally.Add(id, new List<uint>() { obj });
            }
        }
        private static void LoadclanEnemy(uint id, string line)
        {
            Database.DBActions.ReadLine reader = new DBActions.ReadLine(line, '/');
            int count = reader.Read((int)0);
            for (int x = 0; x < count; x++)
            {
                uint obj = reader.Read((uint)0);
                if (clanenemy.ContainsKey(id))
                    clanenemy[id].Add(obj);
                else
                    clanenemy.Add(id, new List<uint>() { obj });
            }
        }
    }
}
