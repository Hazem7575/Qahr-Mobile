using System;
using System.Collections.Concurrent;
using System.IO;
using System.Linq;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using System.Net;

namespace Qahr.Mobile.Base.Security
{
    public class BanInfo
    {
        public DateTime BannedUntil { get; set; }
        public int BanCount { get; set; }
        public string Reason { get; set; }
    }

    public class StrikeInfo
    {
        public int Count { get; set; }
        public int LastSecond { get; set; }
    }

    public static class DosProtection
    {
        // [Security flaw 13] Default widened so normal relog/launcher reconnects are not banned.
        private static int MaxConnectionsPerIpPerSecond = 20;
        private static int TotalMaxConnections = 1000;
        private static int BlockDurationMinutes = 60;
        // [Security flaw 3/13] Concurrent-connection cap per IP (0 = disabled). Generous default so
        // NAT / multi-client households are not harmed.
        private static int MaxActiveConnectionsPerIp = 64;
        // [Security flaw 13] Hard cap on the escalating ban multiplier.
        private static int MaxBanCount = 10;
        // [Security flaw 13] netsh firewall rules are heavy, never auto-removed historically, and block
        // whole machines/NATs. Default OFF — closing the socket already rejects the connection.
        private static bool UseWindowsFirewall = false;
        private static List<string> BlockedIpPrefixes = new List<string>();
        
        private static string ConfigPath = "config.ini";
        private static string BlacklistPath = "blacklist.txt";
        
        private static ConcurrentDictionary<string, StrikeInfo> IpConnectionCounts = new ConcurrentDictionary<string, StrikeInfo>();
        private static ConcurrentDictionary<string, int> ActiveConnectionsPerIp = new ConcurrentDictionary<string, int>();
        private static int TotalConnections = 0;
        private static ConcurrentDictionary<string, BanInfo> Blacklist = new ConcurrentDictionary<string, BanInfo>();
        private static ConcurrentQueue<string> FileQueue = new ConcurrentQueue<string>();
        private static int IsWriting = 0;
        private static int AlertsCount = 0;
        private static DateTime LastSummaryTime = DateTime.UtcNow;

        static DosProtection()
        {
            LoadConfig();
            LoadBlacklist();
            Task.Factory.StartNew(ProcessFileQueue, TaskCreationOptions.LongRunning);
        }

        public static void LoadConfig()
        {
            if (File.Exists(ConfigPath))
            {
                var lines = File.ReadAllLines(ConfigPath);
                foreach (var line in lines)
                {
                    if (line.StartsWith("MaxConnectionsPerIpPerSecond=")) int.TryParse(line.Split('=')[1], out MaxConnectionsPerIpPerSecond);
                    if (line.StartsWith("TotalMaxConnections=")) int.TryParse(line.Split('=')[1], out TotalMaxConnections);
                    if (line.StartsWith("BlockDurationMinutes=")) int.TryParse(line.Split('=')[1], out BlockDurationMinutes);
                    if (line.StartsWith("MaxActiveConnectionsPerIp=")) int.TryParse(line.Split('=')[1], out MaxActiveConnectionsPerIp);
                    if (line.StartsWith("MaxBanCount=")) int.TryParse(line.Split('=')[1], out MaxBanCount);
                    if (line.StartsWith("UseWindowsFirewall=")) bool.TryParse(line.Split('=')[1], out UseWindowsFirewall);
                    if (line.StartsWith("BlockedIpPrefixes=")) 
                    {
                        BlockedIpPrefixes = line.Split('=')[1].Split(',').ToList();
                    }
                }
            }
        }

        public static void LoadBlacklist()
        {
            if (!File.Exists(BlacklistPath)) return;
            
            var lines = File.ReadAllLines(BlacklistPath);
            foreach (var line in lines)
            {
                if (string.IsNullOrEmpty(line)) continue;
                var parts = line.Split('|');
                if (parts.Length < 2) continue;

                string ip = parts[0];
                DateTime expiry;
                if (DateTime.TryParse(parts[1], out expiry))
                {
                    var ban = new BanInfo 
                    { 
                        BannedUntil = expiry, 
                        BanCount = parts.Length > 3 ? int.Parse(parts[3]) : 1, 
                        Reason = parts.Length > 2 ? parts[2] : "Organized DDoS Attack" 
                    };
                    Blacklist.TryAdd(ip, ban);
                }
            }
        }

        public static bool Check(string ip)
        {
            foreach (var prefix in BlockedIpPrefixes)
            {
                if (ip.StartsWith(prefix)) return false; 
            }

            BanInfo ban;
            if (Blacklist.TryGetValue(ip, out ban))
            {
                if (DateTime.UtcNow < ban.BannedUntil)
                {
                    return false;
                }
                else
                {
                    // [Security flaw 13] Ban expired: drop it and remove any firewall rule so the IP
                    // is not blocked forever at the Windows level.
                    BanInfo removed;
                    Blacklist.TryRemove(ip, out removed);
                    RemoveIpFromWindowsFirewall(ip);
                }
            }

            if (Volatile.Read(ref TotalConnections) >= TotalMaxConnections) return false;

            // [Security flaw 3/13] Enforce the per-IP concurrent connection cap (previously tracked
            // but never applied), so a single IP cannot exhaust the global pool.
            if (MaxActiveConnectionsPerIp > 0)
            {
                int active;
                if (ActiveConnectionsPerIp.TryGetValue(ip, out active) && active >= MaxActiveConnectionsPerIp)
                    return false;
            }

            int currentSecond = (int)(DateTime.UtcNow.Ticks / TimeSpan.TicksPerSecond);
            StrikeInfo strike = IpConnectionCounts.GetOrAdd(ip, x => new StrikeInfo { Count = 0, LastSecond = currentSecond });

            lock (strike)
            {
                if (strike.LastSecond != currentSecond)
                {
                    strike.Count = 1;
                    strike.LastSecond = currentSecond;
                }
                else
                {
                    strike.Count++;
                }

                if (strike.Count > MaxConnectionsPerIpPerSecond)
                {
                    AddToBlacklist(ip, "Organized High Rate Connection");
                    return false;
                }
            }

            ActiveConnectionsPerIp.AddOrUpdate(ip, 1, (key, old) => old + 1);
            InterIncrementTotalConnections();
            return true;
        }

        public static void AddToBlacklist(string ip, string reason)
        {
            int currentBanCount = 1;
            BanInfo oldBan;
            if (Blacklist.TryGetValue(ip, out oldBan))
            {
                currentBanCount = oldBan.BanCount + 1;
            }
            // [Security flaw 13] Cap the escalating multiplier so bans can't grow without bound.
            if (MaxBanCount > 0 && currentBanCount > MaxBanCount)
                currentBanCount = MaxBanCount;

            int penaltyMinutes = BlockDurationMinutes * currentBanCount;
            DateTime expiry = DateTime.UtcNow.AddMinutes(penaltyMinutes);

            BanInfo newBan = new BanInfo
            {
                BannedUntil = expiry,
                BanCount = currentBanCount,
                Reason = reason
            };

            Blacklist.AddOrUpdate(ip, newBan, (key, old) => newBan);
            FileQueue.Enqueue(string.Format("{0}|{1}|{2}|{3}", ip, expiry.ToString("o"), reason, currentBanCount));
            if (UseWindowsFirewall)
                BlockIpInWindowsFirewall(ip);
            AlertAdmin(ip, reason);
        }

        // [Security flaw 13] Remove the firewall rule created for an IP when its ban expires.
        public static void RemoveIpFromWindowsFirewall(string ip)
        {
            if (!UseWindowsFirewall) return;
            try
            {
                string ruleName = "AntiDDoS_Block_" + ip.Replace('.', '_');
                string deleteArgs = string.Format("advfirewall firewall delete rule name=\"{0}\"", ruleName);
                System.Diagnostics.ProcessStartInfo deletePsi = new System.Diagnostics.ProcessStartInfo("netsh", deleteArgs);
                deletePsi.WindowStyle = System.Diagnostics.ProcessWindowStyle.Hidden;
                deletePsi.CreateNoWindow = true;
                using (var p = System.Diagnostics.Process.Start(deletePsi))
                {
                    p.WaitForExit();
                }
            }
            catch { }
        }

        public static void BlockIpInWindowsFirewall(string ip)
        {
            try
            {
                string ruleName = "AntiDDoS_Block_" + ip.Replace('.', '_');
                string deleteArgs = string.Format("advfirewall firewall delete rule name=\"{0}\"", ruleName);
                System.Diagnostics.ProcessStartInfo deletePsi = new System.Diagnostics.ProcessStartInfo("netsh", deleteArgs);
                deletePsi.WindowStyle = System.Diagnostics.ProcessWindowStyle.Hidden;
                deletePsi.CreateNoWindow = true;
                using (var p = System.Diagnostics.Process.Start(deletePsi))
                {
                    p.WaitForExit();
                }

                string addArgs = string.Format("advfirewall firewall add rule name=\"{0}\" dir=in action=block remoteip={1}", ruleName, ip);
                System.Diagnostics.ProcessStartInfo addPsi = new System.Diagnostics.ProcessStartInfo("netsh", addArgs);
                addPsi.WindowStyle = System.Diagnostics.ProcessWindowStyle.Hidden;
                addPsi.CreateNoWindow = true;
                System.Diagnostics.Process.Start(addPsi);
            }
            catch { }
        }

        private static void AlertAdmin(string ip, string reason)
        {
            Interlocked.Increment(ref AlertsCount);
            
            if ((DateTime.UtcNow - LastSummaryTime).TotalSeconds >= 10)
            {
                int count = Interlocked.Exchange(ref AlertsCount, 0);
                Console.WriteLine("[DoS PROTECTION] ATTACK SUMMARY: " + count + " IPs blocked in 10s. Last IP: " + ip + " | Reason: " + reason + " | Time: " + DateTime.UtcNow);
                LastSummaryTime = DateTime.UtcNow;
            }
        }
        
        public static void DecrementTotalConnections()
        {
            InterDecrementTotalConnections();
        }

        public static void DisconnectClient(string ip)
        {
            int current;
            if (ActiveConnectionsPerIp.TryGetValue(ip, out current))
            {
                if (current <= 1)
                    ActiveConnectionsPerIp.TryRemove(ip, out current);
                else
                    ActiveConnectionsPerIp.TryUpdate(ip, current - 1, current);
            }
            InterDecrementTotalConnections();
        }

        private static void InterIncrementTotalConnections()
        {
            Interlocked.Increment(ref TotalConnections);
        }

        private static void InterDecrementTotalConnections()
        {
            int current = Volatile.Read(ref TotalConnections);
            if (current > 0)
            {
                Interlocked.Decrement(ref TotalConnections);
            }
        }

        private static void ProcessFileQueue()
        {
            while (true)
            {
                if (!FileQueue.IsEmpty)
                {
                    if (Interlocked.CompareExchange(ref IsWriting, 1, 0) == 0)
                    {
                        List<string> lines = new List<string>();
                        string line;
                        while (FileQueue.TryDequeue(out line))
                        {
                            lines.Add(line);
                        }

                        if (lines.Count > 0)
                        {
                            try
                            {
                                File.AppendAllLines(BlacklistPath, lines);
                            }
                            catch
                            {
                                foreach (var failedLine in lines)
                                {
                                    FileQueue.Enqueue(failedLine);
                                }
                            }
                        }
                        Interlocked.Exchange(ref IsWriting, 0);
                    }
                }
                Thread.Sleep(1000);
            }
        }

        public static void Reload()
        {
            LoadConfig();
            LoadBlacklist();
        }
    }
}
