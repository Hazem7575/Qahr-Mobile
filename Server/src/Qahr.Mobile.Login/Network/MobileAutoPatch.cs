using System;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;

namespace Qahr.Mobile.Login
{
    /// <summary>
    /// QahrMobile: the update check of the Conquer Online Mobile client (AutoUpdateMgr::GetUpdateUrl).
    /// The client connects to autoupdate.ini server1:port1, sends its version number as text and looks for
    /// "READY" (up to date) or "UPDATE" in the reply. We always answer READY, so the client starts without the
    /// "Failed to connect to the update server" dialog. Port: ServerConfig.ini [Server] AutoPatchPort (default 9547,
    /// 0 = off).
    /// </summary>
    public static class MobileAutoPatch
    {
        public static void Start()
        {
            int port = SharedConfig.Int("Server", "AutoPatchPort", 9547);
            if (port <= 0)
                return;
            try
            {
                var listener = new TcpListener(IPAddress.Any, port);
                listener.Start();
                new Thread(() => Loop(listener)) { IsBackground = true, Name = "MobileAutoPatch" }.Start();
                // QahrMobile: [AutoPatch] Mode=Update serves real patches (MobileAutoPatchUpdate.cs); default stays READY.
                bool update = MobileAutoPatchUpdate.Enabled;
                Console.WriteLine("Mobile autopatch on port [" + port + "]" + (update ? " (UPDATE mode)." : " (always READY)."));
                if (update)
                    MobileAutoPatchUpdate.Start();
            }
            catch (Exception e)
            {
                Console.WriteLine("Mobile autopatch port " + port + " failed: " + e.Message);
            }
        }

        const int MaxOpen = 64;
        static readonly SemaphoreSlim Slots = new SemaphoreSlim(MaxOpen);

        static void Loop(TcpListener listener)
        {
            byte[] ready = Encoding.ASCII.GetBytes("READY");
            while (true)
            {
                try
                {
                    var client = listener.AcceptTcpClient();
                    // QahrMobile: at most MaxOpen connections at a time, handled asynchronously, so idle or flooding
                    // connections cannot hold thread-pool threads (the login handling uses the same pool)
                    if (!Slots.Wait(0))
                    {
                        try { client.Close(); } catch { }
                        continue;
                    }
                    System.Threading.Tasks.Task.Run(async () =>
                    {
                        try
                        {
                            if (MobileAutoPatchUpdate.Enabled)// QahrMobile: real UPDATE answers (config flag)
                            {
                                await MobileAutoPatchUpdate.ServeAsync(client);
                                return;
                            }
                            using (client)
                            using (var cts = new CancellationTokenSource(5000))
                            {
                                var s = client.GetStream();
                                byte[] buf = new byte[256];
                                await s.ReadAsync(buf, 0, buf.Length, cts.Token);// the client's version, not needed
                                await s.WriteAsync(ready, 0, ready.Length, cts.Token);
                                await System.Threading.Tasks.Task.Delay(500);// let the client read the reply before the socket closes
                            }
                        }
                        catch { }
                        finally { Slots.Release(); }
                    });
                }
                catch { Thread.Sleep(100); }
            }
        }
    }
}
