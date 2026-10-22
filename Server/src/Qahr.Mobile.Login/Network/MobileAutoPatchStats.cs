using System;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Security.Cryptography;
using System.Text;

namespace Qahr.Mobile.Login
{
    /// <summary>
    /// QahrMobile: statistics of the mobile autopatch (read by Tools\PatchConsole). One line per event in
    /// PatchDir\_stats\downloads.log, tab separated:
    ///   time  kind  ip  device  version  result  bytes  agent
    /// kind "check" = a version question on port 9547 (version = what the phone sent, result = READY or the zip offered),
    /// kind "get" = an HTTP download of NNNN.zip (result ok / fail / notfound, bytes = what was sent).
    /// The IP is written masked (a.b.*.*). "device" is a salted hash of the full IP (salt in _stats\salt, made once),
    /// so the console can count distinct phones without keeping their addresses.
    /// </summary>
    public static class MobileAutoPatchStats
    {
        static string logFile;
        static byte[] salt;
        static readonly object Gate = new object();

        public static void Init(string patchDir)
        {
            try
            {
                string dir = Path.Combine(patchDir, "_stats");
                Directory.CreateDirectory(dir);
                string saltFile = Path.Combine(dir, "salt");
                if (!File.Exists(saltFile))
                    File.WriteAllBytes(saltFile, RandomNumberGenerator.GetBytes(32));
                salt = File.ReadAllBytes(saltFile);
                logFile = Path.Combine(dir, "downloads.log");
            }
            catch (Exception e)
            {
                Console.WriteLine("Mobile autopatch stats off: " + e.Message);
                logFile = null;
            }
        }

        public static void Check(TcpClient c, string version, string reply)
        {
            string r = reply == "READY" ? "READY" : reply.Substring(reply.LastIndexOf('/') + 1);
            Write(c, "check", version, r, 0, "");
        }

        public static void Download(TcpClient c, string name, string result, long from, long size, string[] headers)
        {
            string agent = "";
            foreach (string l in headers)
                if (l.StartsWith("User-Agent:", StringComparison.OrdinalIgnoreCase))
                    agent = l.Substring(11).Trim();
            Write(c, "get", Path.GetFileNameWithoutExtension(name), result, result == "ok" ? size - from : 0, agent);
        }

        static void Write(TcpClient c, string kind, string version, string result, long bytes, string agent)
        {
            if (logFile == null)
                return;
            try
            {
                IPAddress ip = ((IPEndPoint)c.Client.RemoteEndPoint).Address;
                if (ip.IsIPv4MappedToIPv6)
                    ip = ip.MapToIPv4();
                string line = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + "\t" + kind + "\t" + Mask(ip) + "\t" + Device(ip) + "\t" +
                              Clean(version) + "\t" + Clean(result) + "\t" + bytes + "\t" + Clean(agent) + "\r\n";
                lock (Gate)
                    File.AppendAllText(logFile, line);
            }
            catch { }
        }

        static string Mask(IPAddress ip)
        {
            if (ip.AddressFamily == AddressFamily.InterNetwork)
            {
                byte[] b = ip.GetAddressBytes();
                return b[0] + "." + b[1] + ".*.*";
            }
            string[] g = ip.ToString().Split(':');
            return string.Join(":", g, 0, Math.Min(3, g.Length)) + ":*";
        }

        static string Device(IPAddress ip)
        {
            using (var h = new HMACSHA256(salt))
            {
                byte[] d = h.ComputeHash(ip.GetAddressBytes());
                return BitConverter.ToString(d, 0, 6).Replace("-", "").ToLowerInvariant();
            }
        }

        static string Clean(string s)
        {
            s = s ?? "";
            if (s.Length > 80)
                s = s.Substring(0, 80);
            return s.Replace('\t', ' ').Replace('\r', ' ').Replace('\n', ' ');
        }
    }
}
