using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Qahr.Mobile.Database
{
    public class ArenaTable
    {

        internal void Save()
        {
            using (Database.DBActions.Write writer = new DBActions.Write("Arena.ini"))
            {
                foreach (var user in Game.MsgTournaments.MsgArena.ArenaPoll.Values)
                {
                    writer.Add(user.ToString());
                }
                writer.Execute(DBActions.Mode.Open);
            }
        }
        internal void Load()
        {
            using (Database.DBActions.Read reader = new DBActions.Read("Arena.ini"))
            {
                if (reader.Reader())
                {
                    for (int i = 0; i < reader.Count; i++)
                    {
                        Game.MsgTournaments.MsgArena.User user = new Game.MsgTournaments.MsgArena.User();
                        user.Load(reader.ReadString(""));
                        Game.MsgTournaments.MsgArena.ArenaPoll.TryAdd(user.UID, user);
                    }
                }
            }

            Game.MsgTournaments.MsgSchedules.Arena.CreateRankTop10();
            Game.MsgTournaments.MsgSchedules.Arena.CreateRankTop3();
            Game.MsgTournaments.MsgSchedules.Arena.CreateRankTop1000();
            Game.MsgTournaments.MsgArena.UpdateRank();
        }

        // Daily honor by rank, from Infinity[DB]\HonorRewards.ini (same table as the client: "rank=honor/..."),
        // only for players who fought that day. The old "points * 5" gave ~20,000 honor for a single win
        // to anyone off 4000 points, so two accounts could farm unlimited honor.
        private static Dictionary<int, uint> _honorByRank;
        private static Dictionary<int, uint> HonorByRank()
        {
            if (_honorByRank != null) return _honorByRank;
            var table = new Dictionary<int, uint>();
            try
            {
                string file = Program.ServerConfig.DbLocation + "HonorRewards.ini";
                if (System.IO.File.Exists(file))
                {
                    foreach (string raw in System.IO.File.ReadAllLines(file))
                    {
                        string line = raw.Trim();
                        int eq = line.IndexOf('=');
                        if (eq <= 0) continue;
                        int rank;
                        if (!int.TryParse(line.Substring(0, eq), out rank) || rank <= 0) continue;
                        string val = line.Substring(eq + 1);
                        int slash = val.IndexOf('/');
                        uint honor;
                        if (uint.TryParse(slash >= 0 ? val.Substring(0, slash) : val, out honor))
                            table[rank] = honor;
                    }
                }
                else
                    MyConsole.WriteLine("[Arena] HonorRewards.ini not found - no daily arena honor.", ConsoleColor.Yellow);
            }
            catch (Exception e) { MyConsole.WriteLine("[Arena] HonorRewards.ini: " + e.Message, ConsoleColor.Yellow); }
            return _honorByRank = table;
        }

        internal void ResetArena()
        {
            var honorTable = HonorByRank();
            var fought = Game.MsgTournaments.MsgArena.ArenaPoll.Values
                .Where(u => u.Info.TodayWin + u.Info.TodayBattles > 0)
                .OrderByDescending(u => u.Info.ArenaPoints).ThenByDescending(u => u.Info.TodayWin)
                .ToList();
            var dailyRank = new Dictionary<uint, int>();
            for (int i = 0; i < fought.Count; i++)
                dailyRank[fought[i].UID] = i + 1;

            foreach (var user in Game.MsgTournaments.MsgArena.ArenaPoll.Values)
            {
                user.LastSeasonArenaPoints = user.Info.ArenaPoints;
                user.LastSeasonWin = user.Info.TodayWin;
                user.LastSeasonLose = user.Info.TodayBattles;// today's losses
                user.LastSeasonRank = user.Info.TodayRank;



                {
                    int rank;
                    uint honor;
                    if (dailyRank.TryGetValue(user.UID, out rank) && honorTable.TryGetValue(rank, out honor))
                        user.Info.CurrentHonor += honor;
                    if (user.Info.HistoryHonor < user.Info.CurrentHonor)
                        user.Info.HistoryHonor = user.Info.CurrentHonor;
                }

                user.Info.TodayWin = 0;
                user.Info.TodayBattles = 0;

                user.Info.TodayRank = 0;
                user.Info.ArenaPoints = 4000;

            }
            Game.MsgTournaments.MsgSchedules.Arena.CreateRankTop3();
            Game.MsgTournaments.MsgSchedules.Arena.CreateRankTop10();
            Game.MsgTournaments.MsgSchedules.Arena.CreateRankTop1000();
            Game.MsgTournaments.MsgArena.UpdateRank();
        }
    }
}
