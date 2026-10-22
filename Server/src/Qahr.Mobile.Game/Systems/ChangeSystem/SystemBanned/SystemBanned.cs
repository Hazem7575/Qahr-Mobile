using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Qahr.Mobile.Database
{
    public class SystemBanned
    {
        public static System.SafeDictionary<string, Client> BannedPoll = new System.SafeDictionary<string, Client>();
        public class Client
        {
            public string IP = "";
            public uint Hours;
            public long StartBan;

            public override string ToString()
            {

                Database.DBActions.WriteLine writer = new DBActions.WriteLine('/');
                writer.Add(IP).Add(Hours).Add(StartBan);
                return writer.Close();
            }
        }

        public static void AddBan(string IP, uint Hours)
        {
            Client msg = new Client();
            msg.IP = IP;
            msg.Hours = Hours;
            msg.StartBan = DateTime.Now.Ticks;

            BannedPoll.Add(msg.IP, msg);
        }
        public static bool IsBanned(string Ip, out string Messaj)
        {
            if (BannedPoll.ContainsKey(Ip))
            {
                var msg = BannedPoll[Ip];
                if (DateTime.FromBinary(msg.StartBan).AddHours(msg.Hours) < DateTime.Now)
                {
                    BannedPoll.Remove(msg.IP);
                }
                else
                {
                    DateTime receiveban = DateTime.FromBinary(msg.StartBan);
                    DateTime TimerBan = receiveban.AddHours(msg.Hours);
                    TimeSpan time = TimeSpan.FromTicks(TimerBan.Ticks) - TimeSpan.FromTicks(DateTime.Now.Ticks);
                    Messaj = " " + time.Days + " Days " + time.Hours + " Hours " + time.Minutes + " Minutes";
                    return true;
                }
            }
            Messaj = "";
            return false;
        }

        public static void Save()
        {
            using (Database.DBActions.Write writer = new DBActions.Write("BanIp.txt"))
            {
                foreach (var ban in BannedPoll.Values)
                {
                    writer.Add(ban.ToString());
                }
                writer.Execute(DBActions.Mode.Open);
            }
        }

        public static void Load()
        {
            using (Database.DBActions.Read Reader = new DBActions.Read("BanIp.txt"))
            {

                if (Reader.Reader())
                {
                    uint count = (uint)Reader.Count;
                    for (uint x = 0; x < count; x++)
                    {
                        DBActions.ReadLine readline = new DBActions.ReadLine(Reader.ReadString(""), '/');
                        Client msg = new Client();
                        msg.IP = readline.Read((string)"");
                        msg.Hours = readline.Read((uint)0);
                        msg.StartBan = readline.Read((long)0);
                        BannedPoll.Add(msg.IP, msg);
                    }
                }
            }
        }

    }
}
