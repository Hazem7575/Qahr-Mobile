using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;

namespace Qahr.Mobile.Login
{
    /// <summary>
    /// QahrMobile: real resource updates for the Conquer Online Mobile client (AutoUpdateMgr, mode 0).
    /// Off by default: MobileAutoPatch keeps answering READY unless ServerConfig.ini [AutoPatch] Mode=Update.
    ///
    /// Protocol (docs\AUTOPATCH.md): on the same TCP connection the client sends its resource version (first line of
    /// version.dat, e.g. "5150", no terminator) and reads one reply (max 255 bytes):
    ///   "UPDATE host[:port]/path/NNNN.zip"  -> URL "http://host[:port]/path/NNNN.zip"; the client then sends "NNNN"
    ///                                        (the 4 characters before the last '.') and reads the next reply
    ///   "READY"                             -> no (more) update
    /// Up to 50 rounds. Then for each URL: HTTP HEAD (Content-Length), GET (Range when resuming), unzip into the
    /// client's resource root, delete the zip, re-read version.dat: it must now contain NNNN, so every patch zip
    /// carries its own version.dat.
    ///
    /// Patches: [AutoPatch] PatchDir (ServerConfig: D:\Qahr Mobile\Updates) holds NNNN.zip files (4 digits, > the APK's
    /// version.dat 5150, below 9999). A client at version V gets every patch above V, oldest first.
    /// The zips are served by the small HTTP server below ([AutoPatch] HttpPort, default 9549 - 9548 is [Server] MobileRegisterPort; 0 = use your own web
    /// server and set UrlBase).
    /// </summary>
    public static class MobileAutoPatchUpdate
    {
        static readonly Regex PatchName = new Regex(@"^(\d{4})\.zip$", RegexOptions.IgnoreCase);
        static string patchDir;
        static string urlBase;// "host:port/path" without http:// and without a trailing '/'
        static bool started;

        /// <summary>[AutoPatch] Mode=Update turns the real update on; anything else keeps "always READY".</summary>
        public static bool Enabled
        {
            get { return string.Equals(SharedConfig.String("AutoPatch", "Mode", "Ready"), "Update", StringComparison.OrdinalIgnoreCase); }
        }

        /// <summary>Called once by MobileAutoPatch.Start when Enabled.</summary>
        public static void Start()
        {
            if (started)
                return;
            started = true;
            string data = SharedConfig.String("Server", "DataFolder", AppDomain.CurrentDomain.BaseDirectory);
            patchDir = SharedConfig.String("AutoPatch", "PatchDir", Path.Combine(data, "mobile_autopatch"));
            Directory.CreateDirectory(patchDir);
            int httpPort = SharedConfig.Int("AutoPatch", "HttpPort", 9549);
            string ip = SharedConfig.String("Server", "IP", "127.0.0.1");
            urlBase = SharedConfig.String("AutoPatch", "UrlBase", ip + ":" + httpPort + "/patch").Trim();
            if (urlBase.StartsWith("http://", StringComparison.OrdinalIgnoreCase))
                urlBase = urlBase.Substring(7);
            urlBase = urlBase.TrimEnd('/');
            if (urlBase.Contains(" "))
                Console.WriteLine("Mobile autopatch: UrlBase must not contain spaces (the client turns them into '/').");

            var list = Patches();
            Console.WriteLine("Mobile autopatch UPDATE mode: " + list.Count + " patch(es) in " + patchDir + ", URL http://" + urlBase + "/");
            foreach (var p in list)
                Check(p.Value);
            MobileAutoPatchStats.Init(patchDir);
            if (httpPort > 0 && httpPort == SharedConfig.Int("Server", "MobileRegisterPort", 0))
            {
                Console.WriteLine("Mobile autopatch: [AutoPatch] HttpPort " + httpPort + " is the same as [Server] MobileRegisterPort -> HTTP not started, choose another port.");
                httpPort = 0;
            }
            if (httpPort > 0)
                StartHttp(httpPort);
        }

        /// <summary>The patches by version, oldest first (read each time, so a new zip needs no restart).</summary>
        static SortedDictionary<int, string> Patches()
        {
            var r = new SortedDictionary<int, string>();
            try
            {
                foreach (string f in Directory.GetFiles(patchDir, "*.zip"))
                {
                    var m = PatchName.Match(Path.GetFileName(f));
                    int v;
                    if (m.Success && int.TryParse(m.Groups[1].Value, out v) && v < 9999)
                        r[v] = f;
                }
            }
            catch { }
            return r;
        }

        static readonly Dictionary<string, KeyValuePair<DateTime, bool>> valid = new Dictionary<string, KeyValuePair<DateTime, bool>>();

        /// <summary>A zip is served only when it has a version.dat holding its own number; otherwise the client would
        /// download it again on every start. Checked once per file change (a warning is printed for a bad one).</summary>
        static bool Check(string file)
        {
            DateTime t;
            try { t = File.GetLastWriteTimeUtc(file); }
            catch { return false; }
            lock (valid)
            {
                KeyValuePair<DateTime, bool> c;
                if (valid.TryGetValue(file, out c) && c.Key == t)
                    return c.Value;
            }
            string want = Path.GetFileNameWithoutExtension(file), problem = null;
            try
            {
                using (var z = ZipFile.OpenRead(file))
                {
                    var e = z.GetEntry("version.dat");
                    if (e == null)
                        problem = "has no version.dat (must contain " + want + ")";
                    else
                        using (var s = new StreamReader(e.Open()))
                        {
                            string v = (s.ReadLine() ?? "").Trim();
                            if (v != want)
                                problem = "version.dat = '" + v + "', must be " + want;
                        }
                }
            }
            catch (Exception ex) { problem = ex.Message; }
            if (problem != null)
                Console.WriteLine("Mobile autopatch: " + Path.GetFileName(file) + " " + problem + " -> not served");
            lock (valid)
                valid[file] = new KeyValuePair<DateTime, bool>(t, problem == null);
            return problem == null;
        }

        /// <summary>The reply to one version the client sent.</summary>
        public static string Answer(string version)
        {
            int v;
            if (!int.TryParse(version, out v))
                return "READY";
            foreach (var p in Patches())
                if (p.Key > v && Check(p.Value))
                    return "UPDATE " + urlBase + "/" + p.Key.ToString("0000") + ".zip";
            return "READY";
        }

        /// <summary>One autopatch connection (port 9547): answer every version the client sends until READY, the client
        /// closes, or 5 s pass without a request (asynchronous, like MobileAutoPatch, so no pool thread is held).</summary>
        public static async Task ServeAsync(TcpClient client)
        {
            using (client)
            {
                var s = client.GetStream();
                byte[] buf = new byte[256];
                for (int round = 0; round < 60; round++)
                {
                    using (var cts = new CancellationTokenSource(5000))
                    {
                        int n = await s.ReadAsync(buf, 0, buf.Length, cts.Token);
                        if (n <= 0)
                            return;
                        string version = Encoding.ASCII.GetString(buf, 0, n).Trim('\0', ' ', '\r', '\n', '\t');
                        if (version.Length > 16)
                            return;
                        string reply = Answer(version);
                        MobileAutoPatchStats.Check(client, version, reply);// QahrMobile: download statistics
                        byte[] r = Encoding.ASCII.GetBytes(reply);
                        await s.WriteAsync(r, 0, r.Length, cts.Token);
                        if (reply == "READY")
                        {
                            await Task.Delay(500);// let the client read the reply before the socket closes
                            return;
                        }
                    }
                }
            }
        }

        // ------------------------------------------------------------------------------------------- HTTP (zips)
        static void StartHttp(int port)
        {
            try
            {
                var listener = new TcpListener(IPAddress.Any, port);
                listener.Start();
                new Thread(() =>
                {
                    while (true)
                    {
                        try
                        {
                            var c = listener.AcceptTcpClient();
                            if (!HttpSlots.Wait(0))
                            {
                                try { c.Close(); } catch { }
                                continue;
                            }
                            Task.Run(async () =>
                            {
                                try { await HttpAsync(c); }
                                catch { }
                                finally { HttpSlots.Release(); }
                            });
                        }
                        catch { Thread.Sleep(100); }
                    }
                }) { IsBackground = true, Name = "MobileAutoPatchHttp" }.Start();
                Console.WriteLine("Mobile autopatch HTTP on port [" + port + "] (serves " + patchDir + ").");
            }
            catch (Exception e)
            {
                Console.WriteLine("Mobile autopatch HTTP port " + port + " failed: " + e.Message);
            }
        }

        const int MaxHttp = 32;
        static readonly SemaphoreSlim HttpSlots = new SemaphoreSlim(MaxHttp);

        /// <summary>GET/HEAD of /anything/NNNN.zip, with "Range: bytes=N-" (the client resumes broken downloads).
        /// Only NNNN.zip names inside PatchDir are served (no paths). 10 s for the request, 10 min for the transfer.</summary>
        static async Task HttpAsync(TcpClient c)
        {
            using (c)
            using (var cts = new CancellationTokenSource(10000))
            {
                var s = c.GetStream();
                var head = new StringBuilder();
                var one = new byte[1];
                while (!head.ToString().EndsWith("\r\n\r\n"))
                {
                    if (head.Length >= 4096 || await s.ReadAsync(one, 0, 1, cts.Token) <= 0)
                        return;
                    head.Append((char)one[0]);
                }
                cts.CancelAfter(600000);
                string[] lines = head.ToString().Split(new[] { "\r\n" }, StringSplitOptions.RemoveEmptyEntries);
                string[] req = lines[0].Split(' ');
                if (req.Length < 2 || (req[0] != "GET" && req[0] != "HEAD"))
                {
                    await ReplyAsync(s, "405 Method Not Allowed", null, 0, 0, 0, cts.Token);
                    return;
                }
                string path = req[1];
                int q = path.IndexOf('?');
                if (q >= 0)
                    path = path.Substring(0, q);
                string name = path.Substring(path.LastIndexOf('/') + 1);
                string file = PatchName.IsMatch(name) ? Path.Combine(patchDir, name) : null;
                if (file == null || !File.Exists(file))
                {
                    if (req[0] == "GET")
                        MobileAutoPatchStats.Download(c, name, "notfound", 0, 0, lines);
                    await ReplyAsync(s, "404 Not Found", null, 0, 0, 0, cts.Token);
                    return;
                }
                long size = new FileInfo(file).Length, from = 0;
                foreach (string l in lines)
                {
                    var m = Regex.Match(l, @"^Range:\s*bytes=(\d+)-\s*$", RegexOptions.IgnoreCase);
                    long v;
                    if (m.Success && long.TryParse(m.Groups[1].Value, out v))
                        from = v;
                }
                if (from >= size && from > 0)
                {
                    await ReplyAsync(s, "416 Range Not Satisfiable", null, 0, 0, size, cts.Token);
                    return;
                }
                if (req[0] != "GET")
                {
                    await ReplyAsync(s, "200 OK", null, from, size, size, cts.Token);
                    return;
                }
                // QahrMobile: every GET goes to PatchDir\_stats\downloads.log (ok = sent to the end of the file)
                try
                {
                    await ReplyAsync(s, from > 0 ? "206 Partial Content" : "200 OK", file, from, size, size, cts.Token);
                    MobileAutoPatchStats.Download(c, name, "ok", from, size, lines);
                }
                catch
                {
                    MobileAutoPatchStats.Download(c, name, "fail", from, size, lines);
                    throw;
                }
            }
        }

        static async Task ReplyAsync(NetworkStream s, string status, string file, long from, long size, long total, CancellationToken ct)
        {
            bool partial = status.StartsWith("206");
            var h = new StringBuilder();
            h.Append("HTTP/1.1 ").Append(status).Append("\r\n");
            h.Append("Content-Type: application/zip\r\n");
            h.Append("Accept-Ranges: bytes\r\n");
            h.Append("Content-Length: ").Append(size - from).Append("\r\n");
            if (partial)
                h.Append("Content-Range: bytes ").Append(from).Append('-').Append(total - 1).Append('/').Append(total).Append("\r\n");
            else if (status.StartsWith("416"))
                h.Append("Content-Range: bytes */").Append(total).Append("\r\n");
            h.Append("Connection: close\r\n\r\n");
            byte[] hb = Encoding.ASCII.GetBytes(h.ToString());
            await s.WriteAsync(hb, 0, hb.Length, ct);
            if (file == null)
                return;
            using (var f = new FileStream(file, FileMode.Open, FileAccess.Read, FileShare.Read, 64 * 1024, true))
            {
                f.Position = from;
                await f.CopyToAsync(s, 64 * 1024, ct);
            }
        }
    }
}
