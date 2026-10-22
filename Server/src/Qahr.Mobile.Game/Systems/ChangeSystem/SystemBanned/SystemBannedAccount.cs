using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Qahr.Mobile.Database
{
    public class SystemBannedAccount
    {
        public static System.SafeDictionary<uint, Client> BannedPoll = new System.SafeDictionary<uint, Client>();
        public class Client
        {
            public uint UID;
            public string Name;
            public uint Hours;
            public long StartBan;
            public string Reason = "";

            public override string ToString()
            {

                Database.DBActions.WriteLine writer = new DBActions.WriteLine('/');
                writer.Add(UID).Add(Hours).Add(StartBan).Add(Name).Add(Reason);
                return writer.Close();
            }
        }
        public static void AddBan(uint UID, string name, uint Hours, string Reason = "")
        {
            Client msg = new Client();
            msg.UID = UID;
            msg.Hours = Hours;
            msg.Name = name;
            msg.StartBan = DateTime.Now.Ticks;
            msg.Reason = Reason;
            BannedPoll.Add(msg.UID, msg);
        }
        public static void AddBan(Qahr.Mobile.Client.GameClient client, uint Hours, string Reason = "")
        {
            Client msg = new Client();
            msg.UID = client.Player.UID;
            msg.Hours = Hours;
            msg.Name = client.Player.Name;
            msg.StartBan = DateTime.Now.Ticks;
            msg.Reason = Reason;
            BannedPoll.Add(msg.UID, msg);
        }
        public static void RemoveBan(uint UID)
        {
            if (BannedPoll.ContainsKey(UID))
            {
                BannedPoll.Remove(UID);
            }
        }
        public static bool RemoveBanS(uint UID)
        {
            if (BannedPoll.ContainsKey(UID))
            {
                BannedPoll.Remove(UID);
                return true;
            }
            return false;
        }
        public static void RemoveBan(string name)
        {
            uint UID = 0;
            foreach (var obj in BannedPoll.Values)
            {
                if (obj.Name == name)
                {
                    UID = obj.UID;
                    break;
                }
            }
            if (UID != 0)
                RemoveBan(UID);
        }
        public static bool IsBanned(uint UID, out string Messaj)
        {
            if (BannedPoll.ContainsKey(UID))
            {
                var msg = BannedPoll[UID];
                if (DateTime.FromBinary(msg.StartBan).AddHours(msg.Hours) < DateTime.Now)
                {
                    BannedPoll.Remove(msg.UID);
                }
                else
                {
                    DateTime receiveban = DateTime.FromBinary(msg.StartBan);
                    DateTime TimerBan = receiveban.AddHours(msg.Hours);
                    TimeSpan time = TimeSpan.FromTicks(TimerBan.Ticks) - TimeSpan.FromTicks(DateTime.Now.Ticks);
                    Messaj = time.Days + " Days, " + time.Hours + " Hours and " + time.Minutes + " Minutes. (Due to " + msg.Reason + ")";
                    return true;
                }
            }
            Messaj = "";
            return false;
        }

        public static void Save()
        {
            using (Database.DBActions.Write writer = new DBActions.Write("BanUID.txt"))
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
            using (Database.DBActions.Read Reader = new DBActions.Read("BanUID.txt"))
            {

                if (Reader.Reader())
                {
                    uint count = (uint)Reader.Count;
                    for (uint x = 0; x < count; x++)
                    {
                        DBActions.ReadLine readline = new DBActions.ReadLine(Reader.ReadString(""), '/');
                        Client msg = new Client();
                        msg.UID = readline.Read((uint)0);
                        msg.Hours = readline.Read((uint)0);
                        msg.StartBan = readline.Read((long)0);
                        msg.Name = readline.Read("");
                        msg.Reason = readline.Read("");
                        BannedPoll.Add(msg.UID, msg);
                    }
                }
            }
        }
    }
}
