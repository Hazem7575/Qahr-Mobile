using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections.Concurrent;

namespace Qahr.Mobile.Database
{
    public static class ShareVIP
    {
        public static List<Client> SharedPoll = new List<Client>();
        public class Client
        {
            public uint UID;
            public uint ShareUID;
            public string ShareName;
            public byte ShareLevel;
            public DateTime ShareEnds;
            public override string ToString()
            {
                Database.DBActions.WriteLine writer = new DBActions.WriteLine('/');
                writer.Add(UID).Add(ShareUID).Add(ShareName).Add(ShareLevel).Add(ShareEnds.Ticks);
                return writer.Close();
            }
        }
        public static bool CanShare(uint UID, uint ShareUID)
        {
            if (SharedPoll.ToArray().Where(p => p.ShareUID == UID).ToList().Count > 0)
            {
                return false;
            }
            if (SharedPoll.ToArray().Where(p => p.ShareUID == ShareUID).ToList().Count > 0)
            {
                return false;
            }
            if (SharedPoll.ToArray().Where(p => p.UID == ShareUID).ToList().Count > 0)
            {
                return false;
            }
            if (SharedPoll.ToArray().Where(p => p.UID == UID).ToList().Count > 0)
            {
                return false;
            }
            return true;
        }
        public static bool Add(Client x)
        {
            if (CanShare(x.UID, x.ShareUID))
            {
                SharedPoll.Add(x);
                return true;
            }
            return false;
        }
        public static void Save()
        {
            using (Database.DBActions.Write writer = new DBActions.Write("Share.txt"))
            {
                foreach (var x in SharedPoll.ToArray())
                {
                    writer.Add(x.ToString());
                }
                writer.Execute(DBActions.Mode.Open);
            }
        }
        public static void Load()
        {
            using (Database.DBActions.Read Reader = new DBActions.Read("Share.txt"))
            {
                if (Reader.Reader())
                {
                    uint count = (uint)Reader.Count;
                    for (uint i = 0; i < count; i++)
                    {
                        DBActions.ReadLine readline = new DBActions.ReadLine(Reader.ReadString(""), '/');
                        Client x = new Client();
                        x.UID = readline.Read((uint)0);
                        x.ShareUID = readline.Read((uint)0);
                        x.ShareName = readline.Read("");
                        x.ShareLevel = readline.Read((byte)0);
                        x.ShareEnds = DateTime.FromBinary(readline.Read((long)0));
                        SharedPoll.Add(x);
                    }
                }
            }
        }
    }
}
