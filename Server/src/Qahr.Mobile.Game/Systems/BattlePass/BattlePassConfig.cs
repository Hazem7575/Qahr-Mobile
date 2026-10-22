// Martial Token (BattlePass) - season config loader: Infinity[DB]\BattlePass\BattlePass.ini
// The file says, line by line, which values are OFFICIAL (client ini\BattlePass.lua) and which are DERIVED.
using System;
using System.Collections.Generic;
using System.IO;

namespace Qahr.Mobile.BattlePass
{
    public class BpReward
    {
        public int Index, Level, Privilege, Count, Position;
        public uint ItemId;
        public bool Bound;
        public int Group { get { return Position / 100; } }
        public int Bit { get { return Position % 100; } }
    }

    public class BpTask
    {
        public int Index, Type, ReqPrivilege, Score, AutoPrivilege, MultiCost, MultiPrivilege1, MultiPrivilege2, ResetType;
        public ulong Complete;
    }

    public static class BattlePassConfig
    {
        public static string Folder { get { return Path.Combine(Program.ServerConfig.DbLocation, "BattlePass"); } }
        public static string FilePath { get { return Path.Combine(Folder, "BattlePass.ini"); } }

        public static bool Loaded;
        public static uint SeasonId;
        public static bool Enabled;
        public static ulong WeeklyFreePointCap = 6313;
        public static int MultiplyFactor = 2;
        public static bool LoginDaysPerWeek = true;
        public static uint[] Price = new uint[3];
        public static readonly Dictionary<int, ulong> Levels = new Dictionary<int, ulong>();
        public static int MaxLevel = 1;
        public static readonly Dictionary<int, BpReward> Rewards = new Dictionary<int, BpReward>();
        public static readonly SortedDictionary<int, BpTask> Tasks = new SortedDictionary<int, BpTask>();
        public static readonly List<int> RewardGroups = new List<int>();
        /// <summary>Exchange-shop ids paid with Hero Points (client BattlePass.lua ShopInfo.ShopID[0] = 108).</summary>
        public static readonly HashSet<uint> ShopIds = new HashSet<uint>();

        static readonly object sync = new object();

        public static void EnsureLoaded()
        {
            if (Loaded) return;
            lock (sync)
            {
                if (Loaded) return;
                Load();
                Loaded = true;
            }
        }

        static void Load()
        {
            Levels.Clear(); Rewards.Clear(); Tasks.Clear(); RewardGroups.Clear(); ShopIds.Clear();
            SeasonId = 0; Enabled = false; MaxLevel = 1;
            if (!File.Exists(FilePath))
            {
                MyConsole.WriteLine("[BattlePass] config missing: " + FilePath + " (Martial Token disabled)");
                return;
            }
            int line = 0;
            foreach (string raw in File.ReadAllLines(FilePath))
            {
                line++;
                string l = raw;
                int c = l.IndexOf(';');
                if (c >= 0) l = l.Substring(0, c);
                l = l.Trim();
                if (l.Length == 0 || l.StartsWith("[")) continue;
                int eq = l.IndexOf('=');
                if (eq <= 0) continue;
                string key = l.Substring(0, eq).Trim();
                string val = l.Substring(eq + 1).Trim();
                try
                {
                    string[] p = val.Split(',');
                    switch (key.ToLowerInvariant())
                    {
                        case "seasonid": SeasonId = uint.Parse(val); break;
                        case "enabled": Enabled = val == "1" || val.Equals("true", StringComparison.OrdinalIgnoreCase); break;
                        case "weeklyfreepointcap": WeeklyFreePointCap = ulong.Parse(val); break;
                        case "multiplyfactor": MultiplyFactor = Math.Max(1, int.Parse(val)); break;
                        case "cumulativeloginscope": LoginDaysPerWeek = !val.Equals("season", StringComparison.OrdinalIgnoreCase); break;
                        case "price1": Price[1] = uint.Parse(val); break;
                        case "price2": Price[2] = uint.Parse(val); break;
                        case "shopid": ShopIds.Add(uint.Parse(val)); break;
                        case "level":
                            {
                                int lv = int.Parse(p[0]);
                                Levels[lv] = ulong.Parse(p[1]);
                                if (lv > MaxLevel) MaxLevel = lv;
                                break;
                            }
                        case "reward":
                            {
                                var r = new BpReward
                                {
                                    Index = int.Parse(p[0]),
                                    Level = int.Parse(p[1]),
                                    Privilege = int.Parse(p[2]),
                                    ItemId = uint.Parse(p[3]),
                                    Count = int.Parse(p[4]),
                                    Bound = p[5].Trim() != "0",
                                    Position = int.Parse(p[6])
                                };
                                if (r.Group < 1 || r.Group > 13 || r.Bit < 1 || r.Bit > 64 || r.Count < 1 || r.Count > 65535)
                                    throw new Exception("bad reward_position/count");
                                Rewards[r.Index] = r;
                                if (!RewardGroups.Contains(r.Group)) RewardGroups.Add(r.Group);
                                break;
                            }
                        case "task":
                            {
                                var t = new BpTask
                                {
                                    Index = int.Parse(p[0]),
                                    Type = int.Parse(p[1]),
                                    Complete = ulong.Parse(p[2]),
                                    ReqPrivilege = int.Parse(p[3]),
                                    Score = int.Parse(p[4]),
                                    AutoPrivilege = int.Parse(p[5]),
                                    MultiCost = int.Parse(p[6]),
                                    MultiPrivilege1 = int.Parse(p[7]),
                                    MultiPrivilege2 = int.Parse(p[8]),
                                    ResetType = int.Parse(p[9])
                                };
                                Tasks[t.Index] = t;
                                break;
                            }
                    }
                }
                catch (Exception e)
                {
                    MyConsole.WriteLine("[BattlePass] " + FilePath + " line " + line + " ignored: " + e.Message);
                }
            }
            RewardGroups.Sort();
            if (ShopIds.Count == 0) ShopIds.Add(108);
            MyConsole.WriteLine("[BattlePass] season " + SeasonId + (Enabled ? " on" : " off") + ": " + Levels.Count + " levels, "
                + Rewards.Count + " rewards, " + Tasks.Count + " tasks");
        }

        /// <summary>Points needed to go from <paramref name="level"/> to the next one (0 = max level).</summary>
        public static ulong Need(int level)
        {
            ulong n;
            return Levels.TryGetValue(level, out n) ? n : 0;
        }
    }
}
