using System;
using System.Collections.Generic;
using System.IO;

namespace Qahr.Mobile.Database
{
    public class TaskRewards
    {
        public static Dictionary<uint, List<uint>> Rewards = new Dictionary<uint, List<uint>>();
        public static void Load()
        {
            string[] Lines = File.ReadAllLines(Program.ServerConfig.DbLocation + "task_reward_type.ini");
            foreach (var line in Lines)
            {
                if (string.IsNullOrEmpty(line))
                    continue;
                var spilitline = line.Split(new string[] { "@@" }, StringSplitOptions.RemoveEmptyEntries);
                uint UID = uint.Parse(spilitline[0]);
                Rewards.Add(UID, new List<uint>());
                for (int x = 0; x < 8; x++)
                    Rewards[UID].Add(uint.Parse(spilitline[14 + x]));
            }
        }
    }
}
