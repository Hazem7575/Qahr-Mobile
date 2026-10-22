using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;

namespace Qahr.Mobile.ServerSockets
{
    public class BruteForceEntry
    {
        public string IPAddress;
        public int WatchCheck;
        public DateTime Unbantime;
        public DateTime AddedTimeRemove;
    }

    public class BruteforceProtection
    {
        private System.SafeDictionary<string, BruteForceEntry> collection = new System.SafeDictionary<string, BruteForceEntry>();
        private int BanOnWatch;


        private void _internalInit()
        {
            DateTime Now;
            while (true)
            {

                Now = DateTime.Now;
                foreach (BruteForceEntry bfe in collection.GetValues())
                {
                    if (bfe.AddedTimeRemove <= Now)
                    {
                        collection.Remove(bfe.IPAddress);
                    }
                    else if (bfe.Unbantime.Value() != 0)
                    {
                        if (bfe.Unbantime.Value() <= Now.Value())
                        {
                            collection.Remove(bfe.IPAddress);
                        }
                    }
                }

                Thread.Sleep(2000);
            }
        }

        public void Init(int WatchBeforeBan)
        {
            BanOnWatch = WatchBeforeBan;
            new Thread(new ThreadStart(_internalInit)).Start();
        }

        public void AddWatch(string IPAddress)
        {
            lock (collection)
            {
                BruteForceEntry bfe;
                if (!collection.TryGetValue(IPAddress, out bfe))
                {
                    bfe = new BruteForceEntry();
                    bfe.IPAddress = IPAddress;
                    bfe.WatchCheck = 1;
                    bfe.AddedTimeRemove = DateTime.Now.AddMinutes(3);
                    bfe.Unbantime = new DateTime(0);
                    collection.Add(IPAddress, bfe);
                }
                else
                {
                    bfe.WatchCheck++;
                    if (bfe.WatchCheck >= BanOnWatch)
                    {
                        //   Console.WriteLine(IPAddress + "Has been ban");
                        bfe.Unbantime = DateTime.Now.AddMinutes(3);
                    }
                }
            }
        }
        public bool AllowAddress(string IPAddress)
        {
            foreach (var server in Database.GroupServerList.GroupServers.Values)
                if (server.IPAddress == IPAddress)
                    return false;
            return false;
        }
        public bool IsBanned(string IPAddress)
        {
            return false;
            bool check = false;
            BruteForceEntry bfe;
            if (collection.TryGetValue(IPAddress, out bfe))
            {
                check = (bfe.Unbantime.Value() != 0);
            }
            return check;
        }
    }
}