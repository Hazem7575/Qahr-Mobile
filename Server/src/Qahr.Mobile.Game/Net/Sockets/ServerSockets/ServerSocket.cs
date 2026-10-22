using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Net;
using System.Net.Sockets;
using System.IO;
using System.Collections.Concurrent;
using Qahr.Mobile.Base.Security;

namespace Qahr.Mobile.ServerSockets
{
    public class ServerSocket
    {
        private bool Alive = false;
        private Socket Connection;
        public Socket GetConnection
        {
            get { return Connection; }
        }
        public bool IsAlive
        {
            get { return Alive; }
        }
        private string ServerAddresIP = "";
        public uint SPort;
        public string ServerName = "";

        private Action<SecuritySocket> ProcessConnection, ProcessDisconnect;
        private Action<SecuritySocket, Packet> ProcessReceive;

        public ServerSocket(Action<SecuritySocket> _processConnection, Action<SecuritySocket, Packet> _procesreceive, Action<SecuritySocket> _processdisconnect)
        {
            ProcessConnection = _processConnection;
            ProcessReceive = _procesreceive;
            ProcessDisconnect = _processdisconnect;

            Connection = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
        }
        public void Initilize(ushort MaxBufferSend, ushort MaxBufferReceive, uint MaxAcceptConnections, uint MaxClientsConnections)
        {
            Connection.ReceiveBufferSize = MaxBufferReceive;
            Connection.SendBufferSize = MaxBufferSend;
        }
        public void Connect(string IpAddres, ushort port, string aServerName)
        {
            SPort = port;
            ServerAddresIP = IpAddres;
            ServerName = aServerName;
            TryConnect(aServerName);
        }
        private void TryConnect(string servername)
        {
            Connection.BeginConnect(ServerAddresIP, (int)SPort, new AsyncCallback(ConnectCallback), null);
        }
        private void ConnectCallback(IAsyncResult ar)
        {
            try
            {
                Connection.EndConnect(ar);
                MyConsole.WriteLine("Server has been connected to InterServer successfully!", ConsoleColor.DarkRed);
                Connection.SetSocketOption(SocketOptionLevel.Socket, SocketOptionName.DontLinger, true);
                Connection.SetSocketOption(SocketOptionLevel.Socket, SocketOptionName.OutOfBandInline, true);
                Connection.ReceiveBufferSize = 8192;
                Connection.SendBufferSize = 8192;
                Alive = Connection.Connected;
                if (Alive)
                {
                    var client = new SecuritySocket(this, ProcessDisconnect, ProcessReceive);
                    client.Create(Connection);
                    if (ProcessConnection != null)
                    {
                        ProcessConnection(client);
                    }
                }
            }
            catch
            {
                TryConnect(ServerName);
            }
        }
        // [net V2] Which I/O engine the sockets accepted by this listener use:
        //   Legacy (default) = the old 1ms select() polling on Thread4K.ReceivePool + one BeginSend per packet.
        //   V2               = async receive (SocketAsyncEventArgs, no polling) + one ordered send queue per
        //                      connection (see SecuritySocket.V2.cs).
        // Chosen by ServerConfig.ini [Network] NetEngine=Legacy|V2, read once in Open(). Set NetEngine before
        // Open() to force it (test harness). Outgoing (InterServer) connections always stay Legacy.
        public string NetEngine = null;
        public bool EngineV2 { get; private set; }

        public void Open(string IpAddres, ushort port, int backlog)
        {
            try
            {
                string engine = NetEngine ?? Qahr.Mobile.SharedConfig.String("Network", "NetEngine", "Legacy");
                EngineV2 = string.Equals((engine ?? "").Trim(), "V2", StringComparison.OrdinalIgnoreCase);
                if (EngineV2)
                    SecuritySocket.LoadV2Settings();
                MyConsole.WriteLine("[Network] NetEngine=" + (EngineV2 ? "V2 (async receive + send queue)" : "Legacy") + " on port " + port, ConsoleColor.White);
            }
            catch (Exception e)
            {
                EngineV2 = false;
                MyConsole.SaveException(e);
            }
            SPort = port;
            ServerAddresIP = IpAddres;
            Connection.Bind(new IPEndPoint(IPAddress.Any, port));
            Connection.Listen(1000);
            Connection.Blocking = false;
            Connection.SetSocketOption(SocketOptionLevel.Socket, SocketOptionName.ReuseAddress, true);
            Connection.SetSocketOption(SocketOptionLevel.Tcp, SocketOptionName.NoDelay, true);
            Connection.SetSocketOption(SocketOptionLevel.Socket, SocketOptionName.DontLinger, true);
            Connection.SetSocketOption(SocketOptionLevel.Socket, SocketOptionName.KeepAlive, true);
            Connection.SetSocketOption(SocketOptionLevel.Socket, SocketOptionName.OutOfBandInline, true);

            Alive = true;
            Connection.BeginAccept(Handle_Accept, null);
        }
        public void Handle_Accept(IAsyncResult result)
        {
            try
            {
                if (Alive)
                {
                    var socket = Connection.EndAccept(result);
                    string RemoteIP = (socket.RemoteEndPoint as IPEndPoint).Address.ToString();

                    if (!DosProtection.Check(RemoteIP))
                    {
                        try
                        {
                            socket.LingerState = new LingerOption(true, 0);
                            socket.Shutdown(SocketShutdown.Both);
                        }
                        catch { }
                        finally
                        {
                            socket.Close();
                        }
                        return; // إعادة الاستماع بتحصل في finally تحت
                    }

                    SecuritySocket user = new SecuritySocket(this, ProcessDisconnect, ProcessReceive);
                    // This socket was counted by DosProtection.Check above; mark it so its Disconnect()
                    // releases the count exactly once (prevents the TotalConnections leak that blocked logins after ~1000).
                    user.DosCounted = true;
                    if (EngineV2)
                    {
                        // [net V2] Must be set before Create(): Create() then skips the 1ms receive timer.
                        user.UseV2 = true;
                        try { socket.NoDelay = true; } catch { }
                    }
                    user.Create(socket);
                    if (ProcessConnection != null)
                    {
                        ProcessConnection.Invoke(user);
                    }
                    user.ConnectFull = true;
                    // [net V2] Start the async receive loop only now (same point where the Legacy poller
                    // starts doing work: after the GameClient exists and ConnectFull is set).
                    if (user.UseV2)
                        user.StartReceiveV2();
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e.ToString());
                MyConsole.SaveException(e);
            }
            finally
            {
                // لازم نرجع نستمع دايمًا — قبل كده أي exception كان بيوقف قبول اللاعبين الجدد والسيرفر لسه شغّال.
                if (Alive)
                {
                    try { Connection.BeginAccept(Handle_Accept, null); }
                    catch (Exception e2) { MyConsole.SaveException(e2); }
                }
            }
        }
        public void CloseConnection(Socket socket)
        {
            try
            {
                if (socket != null && socket.RemoteEndPoint != null)
                {
                    string RemoteIP = ((IPEndPoint)socket.RemoteEndPoint).Address.ToString();
                    DosProtection.DisconnectClient(RemoteIP);
                }
                else
                {
                    DosProtection.DecrementTotalConnections();
                }
            }
            catch
            {
                DosProtection.DecrementTotalConnections();
            }

            // [perf N1c] Was shutdown + closesocket(Handle) + Dispose(): the handle was closed twice (Dispose
            // closes the SafeHandle again), which could close a newly accepted socket that got the same
            // handle value. Now: Shutdown (may throw on a listener / unconnected socket) then Dispose only.
            SocketHelpers.ShutdownAndDispose(socket);
            if (socket != null)
                GC.SuppressFinalize(socket);
        }
        public void Close()
        {
            if (Alive)
            {
                Alive = false;
                CloseConnection(Connection);
            }
        }
    }
}
