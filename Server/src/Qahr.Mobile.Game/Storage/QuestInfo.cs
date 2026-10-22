using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Qahr.Mobile.Database
{
    public class QuestInfo
    {
        /*[1]
TypeId=1
TaskNameColor=0xFFFFFFFF
CompleteFlag=2
ActivityType=0
MissionId=650
Name=The Armorer 
Lv_min=8

*/
        public class DBQuest
        {
            public class NpcInfo
            {
                public uint ID;
                public ushort Map;
                public ushort X;
                public ushort Y;
                public string Name;
                public string MapName;

                public NpcInfo(string line)
                {
                    if (line == "")
                        return;
                    try
                    {
                        string[] data = line.Split(',');
                        ID = uint.Parse(data[0]);
                        Map = ushort.Parse(data[1]);
                        X = ushort.Parse(data[2]);
                        Y = ushort.Parse(data[3]);
                        Name = data[4];
                        MapName = data[5];
                    }
                    catch
                    {
                    }
                }
            }
            public byte TypeId = 0;
            public byte TypeBigId = 0;
            public byte CompleteFlag = 0;
            public uint ActivityType = 0;
            public uint MissionId = 0;
            public string Name = "";
            public ushort Lv_min;
            public ushort Lv_max;
            public ushort First;
            // public ushort Prequest;
            public ushort Map;
            public uint[] Profession;
            public ulong Exp = 0;
            public ulong Gold = 0;
            public byte Sex;
            public uint FinishTime;
            public uint ActivityBeginTime;
            public uint ActivityEndTime;
            public NpcInfo BeginNpcId;
            public NpcInfo FinishNpcId;
            public uint Intentions;
            public uint[] Prize;
            public uint[] Prequest;// [QuestFix 2026-10-13] was ushort[] (590xxx/9xxxx ids overflowed)
            // [QuestFix 2026-10-13] client semantics: "a|b|c" = ANY one finished; "a,b,c" (or "a&b") = ALL finished
            public bool PrequestAny;
            public bool CanAcceptQuest(uint Class)
            {
                if (Class % 100 > 5)
                    Class = ((Class / 1000) * 1000) + 5;
                if (Profession.Length == 0 || Profession.Length == 1)
                    return true;
                for (int x = 0; x < Profession.Length; x++)
                    if (Class == Profession[x])
                        return true;
                return false;
            }

        }
        public class ListNpcQuests
        {
            public List<DBQuest> Quests = new List<DBQuest>();
        }
        public static Dictionary<uint, DBQuest> AllQuests = new Dictionary<uint, DBQuest>();
        public static Dictionary<uint, ListNpcQuests> NpcQuests = new Dictionary<uint, ListNpcQuests>();

        // [QuestFix 2026-10-13] safe lookup by MissionId: never throws and never returns null, so an NPC
        // script that names a mission missing from Questinfo.ini can't crash its dialog. The placeholder
        // isn't added to AllQuests (it can't be accepted from the quest panel).
        public static DBQuest GetQuest(uint QuestID)
        {
            DBQuest quest;
            if (AllQuests.TryGetValue(QuestID, out quest))
                return quest;
            if (QuestID != 0)
                MyConsole.WriteLine("[QuestInfo] mission " + QuestID + " is not in Questinfo.ini (placeholder used).");
            return new DBQuest()
            {
                MissionId = QuestID,
                Prequest = new uint[1] { 0 },
                Profession = new uint[1] { 0 },
                Prize = new uint[0],
                BeginNpcId = new DBQuest.NpcInfo(""),
                FinishNpcId = new DBQuest.NpcInfo("")
            };
        }
        public static DBQuest GetFinishQuest(uint NPCID, uint _class, uint QuestID = 0, Func<DBQuest, bool> P = null)
        {
            ListNpcQuests array;// [QuestFix 2026-10-13] was NpcQuests[NPCID] (KeyNotFound when the NPC begins no quest)
            if (!NpcQuests.TryGetValue(NPCID, out array))
                return QuestID != 0 ? GetQuest(QuestID) : null;
            for (int x = 0; x < array.Quests.Count; x++)
            {
                var quest = array.Quests[x];

                if (QuestID == 0)
                {
                    if (quest.CanAcceptQuest(_class))
                    {
                        if (P != null)
                        {
                            if (P(quest))
                                return quest;
                        }
                        else
                            return quest;
                    }
                }
                else if (quest.MissionId == QuestID)
                {
                    return quest;
                }

            }
            return QuestID != 0 ? GetQuest(QuestID) : null;// [QuestFix 2026-10-13] explicit id: never null (was null -> NullReference in the dialog)
        }
        public static List<uint> KingDomMissions = new List<uint>() { 35024, 35007, 35025, 35028, 35034 };
        public static bool IsKingDomMission(uint MissionID)
        {
            return KingDomMissions.Contains(MissionID);

        }


        // [QuestFix 2026-10-13] parses "a|b|c" (ANY) or "a,b,c" / "a&b" (ALL); bad tokens are skipped, never thrown.
        public static uint[] ParseIdList(string text, out bool any)
        {
            any = text.IndexOf('|') >= 0;
            var list = new List<uint>();
            foreach (var token in text.Split(new char[] { '|', ',', '&' }, StringSplitOptions.RemoveEmptyEntries))
            {
                uint value;
                if (uint.TryParse(token.Trim(), out value))
                    list.Add(value);
            }
            if (list.Count == 0)
                list.Add(0);
            return list.ToArray();
        }

        // [QuestFix 2026-10-13] the numeric [N] sections actually present in the file, in ascending order.
        // The old loop "for (count = 1; count < TotalMission; ...)" skipped the last section and relied on
        // contiguous numbering; now every numeric section is loaded whatever TotalMission says.
        static List<uint> QuestSections(WindowsAPI.IniFile reader, int TotalMission)
        {
            var sections = new List<uint>();
            try
            {
                foreach (var raw in System.IO.File.ReadLines(reader.FileName, WindowsAPI.IniFile.FileEncoding))
                {
                    var line = raw.Trim();
                    if (line.Length < 3 || line[0] != '[')
                        continue;
                    int close = line.IndexOf(']');
                    uint number;
                    if (close > 1 && uint.TryParse(line.Substring(1, close - 1).Trim(), out number) && !sections.Contains(number))
                        sections.Add(number);
                }
            }
            catch (Exception e)
            {
                MyConsole.WriteLine("[QuestInfo] could not scan sections: " + e.Message);
            }
            if (sections.Count == 0)
                for (uint count = 1; count <= TotalMission; count++)
                    sections.Add(count);
            sections.Sort();
            return sections;
        }

        public static void Init()
        {
            // [QuestEngine] build into new tables and publish them at the end (reference swap), so "reloadquests"
            // can re-run Init on a live server: readers see the old or the new tables, never a half-filled one.
            var AllQuests = new Dictionary<uint, DBQuest>();
            var NpcQuests = new Dictionary<uint, ListNpcQuests>();
            WindowsAPI.IniFile reader = new WindowsAPI.IniFile("\\Questinfo.ini");
            int TotalMission = reader.ReadUInt16("TotalMission", "TotalMission", 0);
            var sections = QuestSections(reader, TotalMission);
            if (TotalMission != sections.Count)
                MyConsole.WriteLine("[QuestInfo] TotalMission=" + TotalMission + " but the file has " + sections.Count + " quest sections; loading all sections.");

            foreach (uint sectionNo in sections)
            {
                string sec = sectionNo.ToString();
                DBQuest quest = new DBQuest();
                quest.MissionId = reader.ReadUInt32(sec, "MissionId", 0);
                if (quest.MissionId == 0)// [QuestFix 2026-10-13] empty/broken section
                    continue;
                quest.TypeId = reader.ReadByte(sec, "TypeId", 0);
                quest.TypeBigId = reader.ReadByte(sec, "TypeBigId", 0);
                
                quest.CompleteFlag = reader.ReadByte(sec, "CompleteFlag", 0);
                quest.ActivityType = reader.ReadByte(sec, "ActivityType", 0);
                quest.Intentions = reader.ReadUInt16(sec, "IntentAmount", 0);
                // [QuestFix 2026-10-13] level range was never read, so CanClientAcceptQuest's level check never ran (0 = no bound)
                quest.Lv_min = reader.ReadUInt16(sec, "Lv_min", 0);
                quest.Lv_max = reader.ReadUInt16(sec, "Lv_max", 0);
                quest.Prequest = ParseIdList(reader.ReadBigString(sec, "Prequest", ""), out quest.PrequestAny);

                bool unused;
                quest.Profession = ParseIdList(reader.ReadBigString(sec, "Profession", ""), out unused);

                string Prize = reader.ReadBigString(sec, "Prize", "");
                if (Prize.Contains('['))
                {

                    string[] items = Prize.Split('[');
                    quest.Prize = new uint[items.Length - 1];
                    try
                    {
                        for (int x = 1; x < items.Length; x++)
                        {
                            var element = items[x];
                            var sp1 = element.Split(' ');
                            var sp2 = sp1[1].Split(',');
                            quest.Prize[x - 1] = uint.Parse(sp2[0]);
                        }
                    }
                    catch
                    {

                    }
                }
                quest.BeginNpcId = new DBQuest.NpcInfo(reader.ReadString(sec, "BeginNpcId", ""));
                quest.FinishNpcId = new DBQuest.NpcInfo(reader.ReadString(sec, "FinishNpcId", ""));
  
                if (!AllQuests.ContainsKey(quest.MissionId))
                    AllQuests.Add(quest.MissionId, quest);
               
                if (NpcQuests.ContainsKey(quest.BeginNpcId.ID))
                {
                    NpcQuests[quest.BeginNpcId.ID].Quests.Add(quest);
                }
                else
                {
                    NpcQuests.Add(quest.BeginNpcId.ID, new ListNpcQuests());
                    NpcQuests[quest.BeginNpcId.ID].Quests.Add(quest);
                }

            }
            QuestInfo.AllQuests = AllQuests;// [QuestEngine] publish
            QuestInfo.NpcQuests = NpcQuests;
        }
    }
}

