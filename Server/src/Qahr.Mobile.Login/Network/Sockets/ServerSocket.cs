// * Created by ElmistRo
// * Copyright © 2010-2014
// * ElmistRo - Project

using System;
using System.IO;
using System.Net;
using System.Text;
using System.Threading;
using System.Net.Sockets;
using System.Collections.Generic;
using System.Collections.Concurrent;

namespace Qahr.Mobile.Login.Network.Sockets
{
    public unsafe class ServerSocket
    {
        public event Action<ClientWrapper> OnClientConnect, OnClientDisconnect;
        public event Action<byte[], int, ClientWrapper> OnClientReceive;
       
        private object SyncRoot;
        private Socket Connection;
        private ushort port;
        private string ipString;
        private bool enabled;
        private System.Threading.Thread thread;

        // [Security flaw 5] Concurrent connection cap per source IP (0 = disabled). Set from Shell.ini.
        public static int MaxConnectionsPerIp = 32;
        private static readonly System.Collections.Concurrent.ConcurrentDictionary<string, int> ActivePerIp
            = new System.Collections.Concurrent.ConcurrentDictionary<string, int>();
        public ServerSocket()
        {
            this.Connection = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            this.SyncRoot = new object();
            thread = new System.Threading.Thread(doSyncAccept);
            thread.Start();
        }
        public void Enable(ushort port, string ip)
        {
            this.ipString = ip;
            this.port = port;
            this.Connection.Bind(new IPEndPoint(IPAddress.Parse(ipString), this.port));
            this.Connection.Listen(50);
            this.enabled = true;
            
        }
        public bool PrintoutIPs = true;
        private void doSyncAccept()
        {
            while (true)
            {
                if (this.enabled)
                {
                    try
                    {
                        processSocket(this.Connection.Accept());
                    }
                    catch
                    {
                    }
                }
            }
        }
        private void doAsyncAccept(IAsyncResult res)
        {
            try
            {
                Socket socket = this.Connection.EndAccept(res);
                processSocket(socket);
            }
            catch
            {
            }
            finally
            {
                // لازم نرجّع الاستماع دايمًا، وإلا exception واحد (RST قبل الـ accept) يوقف الدخول خالص
                try { this.Connection.BeginAccept(doAsyncAccept, null); }
                catch (ObjectDisposedException) { }
                catch (Exception e) { Console.WriteLine("[Accept] re-arm failed: " + e.Message); }
            }
        }
        private void processSocket(Socket socket)
        {
            try
            {
                string ip = (socket.RemoteEndPoint as IPEndPoint).Address.ToString();

                // [Security flaw 5] Reject if this IP already holds too many concurrent auth sockets.
                if (MaxConnectionsPerIp > 0)
                {
                    int active;
                    if (ActivePerIp.TryGetValue(ip, out active) && active >= MaxConnectionsPerIp)
                    {
                        Console.WriteLine("[Auth] " + DateTime.Now.ToString("HH:mm:ss") + " " + ip + " REJECTED: " + active + " open auth sockets from this IP (max " + MaxConnectionsPerIp + ")");
                        try { System.IO.File.AppendAllText(System.IO.Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "auth_log.txt"), DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + " " + ip + " [] REJECTED: " + active + " open auth sockets from this IP (max " + MaxConnectionsPerIp + ")" + Environment.NewLine); } catch { }
                        try { socket.Shutdown(SocketShutdown.Both); } catch { }
                        try { socket.Close(); } catch { }
                        return;
                    }
                }
                ActivePerIp.AddOrUpdate(ip, 1, (k, old) => old + 1);

                ClientWrapper wrapper = new ClientWrapper();
                wrapper.Create(socket, this, OnClientReceive);
                wrapper.Alive = true;
                wrapper.IP = ip;
                if (this.OnClientConnect != null) this.OnClientConnect(wrapper);
            }
            catch
            {
            }
        }
        public void Reset()
        {
            this.Disable();
            this.Enable();
        }
        public void Disable()
        {
            this.enabled = false;
            this.Connection.Close(1);
        }
        public void Enable()
        {
            if (!this.enabled)
            {
                this.Connection = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
                this.Connection.Bind(new IPEndPoint(IPAddress.Parse(ipString), this.port));
                this.Connection.Listen((int)SocketOptionName.MaxConnections);
                this.enabled = true;
            }
        }
        public void InvokeDisconnect(ClientWrapper Client)
        {
            // [Security flaw 5] Release this IP's concurrent-connection slot exactly once.
            try
            {
                if (Client != null && !string.IsNullOrEmpty(Client.IP) && Client.Alive)
                {
                    string ip = Client.IP;
                    int cur;
                    if (ActivePerIp.TryGetValue(ip, out cur))
                    {
                        if (cur <= 1) ActivePerIp.TryRemove(ip, out cur);
                        else ActivePerIp.TryUpdate(ip, cur - 1, cur);
                    }
                }
            }
            catch { }
            if (this.OnClientDisconnect != null)
                this.OnClientDisconnect(Client);
        }
        public bool Enabled
        {
            get { return this.enabled; }
        }
    }
}