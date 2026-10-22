using System;
using System.Collections.Generic;
using System.IO;

namespace Qahr.Mobile.Database
{
    public class syndicate_level
    {
        public class Level
        {
            public uint level;
            public uint Max_Member;
            public uint Max_DeputyLeader;
            public uint Max_Manager;
            public uint Max_Steward;
        }
        public static Dictionary<uint, syndicate_level.Level> _syndicate_level;

        public static void Load()
        {
            if (!File.Exists(Program.ServerConfig.DbLocation + "syndicate_level.txt"))
            {
                Console.WriteLine("!File.Exists: " + Program.ServerConfig.DbLocation + "syndicate_level.txt");
            }
            else
            {
                syndicate_level._syndicate_level = new Dictionary<uint, syndicate_level.Level>();
                foreach (string readAllLine in File.ReadAllLines(Program.ServerConfig.DbLocation + "syndicate_level.txt"))
                {
                    string[] separator = new string[2] { "@@", " " };
                    string[] strArray = readAllLine.Split(separator, StringSplitOptions.RemoveEmptyEntries);
                    syndicate_level.Level level = new syndicate_level.Level();
                    level.level = Convert.ToUInt32(strArray[0]);
                    level.Max_Member = Convert.ToUInt32(strArray[1]);
                    level.Max_DeputyLeader = Convert.ToUInt32(strArray[2]);
                    level.Max_Manager = Convert.ToUInt32(strArray[3]);
                    level.Max_Steward = Convert.ToUInt32(strArray[4]);
                    syndicate_level._syndicate_level.Add(level.level, level);
                }
            }
        }


    }
}