using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace Qahr.Mobile.Database
{
    public class TutorInfo
    {
        public class TutorType
        {
            public int Index;
            public int MinLevel;
            public int MaxLevel;
            public int StudentNum;
            public int BattleLevShare;
        }
        public static List<TutorType> TutorTypes = new List<TutorType>();
        public static int[] BattleLimit = new int[400];

        public static void Load()
        {
            string[] baseText = File.ReadAllLines(Program.ServerConfig.DbLocation + "cq_tutor_type.txt");
            foreach (var bas_line in baseText)
            {
                if (bas_line.Contains(';')) continue;
                string[] line = bas_line.Split(',');
                TutorType obj = new TutorType();
                obj.Index = int.Parse(line[0]);
                obj.MinLevel = int.Parse(line[1]);
                obj.MaxLevel = int.Parse(line[2]);
                obj.StudentNum = int.Parse(line[3]);
                obj.BattleLevShare = int.Parse(line[4]);
                TutorTypes.Add(obj);
            }
            baseText = File.ReadAllLines(Program.ServerConfig.DbLocation + "cq_tutor_battle_limit_type.txt");
            BattleLimit = new int[baseText.Length + 1];
            foreach (var bas_line in baseText)
            {
                if (bas_line.Contains(';')) continue;
                string[] line = bas_line.Split(',');
                BattleLimit[int.Parse(line[0])] = int.Parse(line[1]);

            }
        }

        public static TutorType GetTutorInfo(Client.GameClient user)
        {
            foreach (var obj in TutorTypes)
            {
                if (user.Player.Level >= obj.MinLevel && Math.Min(140, (int)user.Player.Level) <= obj.MaxLevel)
                    return obj;
            }
            return null;
        }
        public static int AddAppCount(Client.GameClient user)
        {
           var info = GetTutorInfo(user);
           if (info != null)
               return info.StudentNum;
            return 0;
        }

        public static int ShareBattle(Client.GameClient Mentor, int Student_battle)
        {
            if (Student_battle >= Mentor.Player.RealBattlePower)
                return 0;
            if (Mentor.Player.Class >= 2005 && Mentor.Player.Class >= 2051)
                return 0;
            var Tutor = GetTutorInfo(Mentor);
            if (Tutor != null)
            {
                int battle_lev_share = Tutor.BattleLevShare;
                int share = (Mentor.Player.RealBattlePower - Student_battle) * battle_lev_share / 100;
                int share_limit = BattleLimit[Math.Min(BattleLimit.Length - 1, Student_battle)];
                return Math.Min(share, share_limit);
            }
            return 0;
        }

    }
}
