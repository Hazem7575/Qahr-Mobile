using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections.Concurrent;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using System.Runtime.InteropServices;
using System.Diagnostics;
using System.Net.NetworkInformation;

namespace Qahr.Mobile.ServerSockets
{
    public unsafe partial class SecuritySocket
    {
        public ReceiveBuffer ReceiveBuff;
        public bool IsGameServer
        {
            get
            {
                return Crypto != null;
            }
        }

        public bool SetDHKey = false;
        public bool IsBot = false;
        public object SendRoot;
        public bool SetAfterConnectKey = false;

        private Action<SecuritySocket> OnDisconnect;
        private Action<SecuritySocket, Packet> OnReceiveHandler;
        public Socket Connection;
        public object Client;

        private IDisposable[] TimerSubscriptions = null;
        public bool Alive = false;
        private Queue<byte[]> SendQueue;
        public Cryptography.TQCast5 Crypto;
        public bool OnInterServer = false, OverrideTiming = false;
        public string RemoteIp
        {
            get
            {
                try
                {
                    if (Connection == null)
                        return "NONE";
                    return (Connection.RemoteEndPoint as System.Net.IPEndPoint).Address.ToString();
                }
                catch
                {
                    return "NONE";
                }
            }
        }
        public Time32 LastReceive, LastReceiveCall;
        public Client.GameClient Game;
        public ServerSocket Server;

        public bool ConnectFull = false;
        // True only for sockets that passed DosProtection.Check on accept (so exactly those get decremented on disconnect).
        public bool DosCounted = false;

        public SecuritySocket(ServerSocket serversocket, Action<SecuritySocket> _OnDisconnect, Action<SecuritySocket, Packet> _OnReceiveHandler)
        {
            Server = serversocket;
            OnReceiveHandler = _OnReceiveHandler;
            OnDisconnect = _OnDisconnect;


        }
        public SecuritySocket(Action<SecuritySocket> _OnDisconnect, Action<SecuritySocket, Packet> _OnReceiveHandler)
        {
            OnReceiveHandler = _OnReceiveHandler;
            OnDisconnect = _OnDisconnect;
        }
        public bool Connect(string IPAddres, ushort port, out Socket _socket)
        {
            _socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            IAsyncResult asyncResult = _socket.BeginConnect(new IPEndPoint(IPAddress.Parse(IPAddres), port), null, null);
            uint count = 0;
            while (!asyncResult.IsCompleted && count < 10)
            {
                count++;
                System.Threading.Thread.Sleep(100);
            }
            if (asyncResult.IsCompleted)
            {

                _socket.Blocking = false;
                _socket.SetSocketOption(SocketOptionLevel.Socket, SocketOptionName.ReuseAddress, true);
                _socket.SetSocketOption(SocketOptionLevel.Tcp, SocketOptionName.NoDelay, true);
                _socket.SetSocketOption(SocketOptionLevel.Socket, SocketOptionName.DontLinger, true);

                // Set option that allows socket to receive out-of-band information in the data stream. 
                _socket.SetSocketOption(SocketOptionLevel.Socket, SocketOptionName.OutOfBandInline, true);
                _socket.ReceiveBufferSize = _socket.SendBufferSize = ServerSockets.ReceiveBuffer.RECV_BUFFER_SIZE;

            }

            return !((_socket.Poll(1000, SelectMode.SelectRead) && (_socket.Available == 0)) || !_socket.Connected);
            //return asyncResult.IsCompleted;
        }
        public void Create(Socket _socket)
        {
            try
            {
                ReceiveBuff = new ReceiveBuffer(Qahr.Mobile.Program.ServerConfig.Port_ReceiveSize);
                Connection = _socket;
                SetDHKey = false;
                Alive = true;
                LastReceive = Time32.Now;
                SendRoot = new object();
                Client = Crypto = null;
                if (UseV2)
                {
                    // [net V2] No 1ms poll timer: StartReceiveV2() posts an async receive instead, and the
                    // 5s TryReview runs from one shared timer for all V2 sockets (SecuritySocketV2.cs).
                    CreateV2();
                    return;
                }
                TimerSubscriptions = new IDisposable[]
                {
                    Thread4K.Subscribe<SecuritySocket>(Program.CallBack.ConnectionReceive, this, Program.CallBack.ReceivePool),
                    // Idle/receive timeout: periodically run TryReview so dead sockets are disconnected.
                    Thread4K.Subscribe<SecuritySocket>(ReviewCallBack, this, 5000, ThreadPriority.Normal)
                };
            }
            catch (Exception e)
            {
                MyConsole.SaveException(e);
            }
        }
        public void SetCrypto(Cryptography.TQCast5 Crypt)
        {
            if (!Program.ServerConfig.IsInterServer)
            {
                Crypto = Crypt;
            }
        }
        public static void TryReview(SecuritySocket wrapper)
        {
            if (wrapper.Crypto != null && wrapper.Alive)
            {
                // [net V2] Legacy refreshes LastReceiveCall on every 1ms poll ("the poller is running").
                // V2 has a receive posted all the time, so it is always "running".
                if (wrapper.UseV2)
                    wrapper.LastReceiveCall = Time32.Now;
                if (wrapper.OverrideTiming)
                {
                    if (Time32.Now > wrapper.LastReceive.AddSeconds(60 * 3))
                        wrapper.Disconnect();
                }
                else
                {
                    if (Time32.Now < wrapper.LastReceiveCall.AddSeconds(2))
                        if (Time32.Now > wrapper.LastReceive.AddSeconds(60))
                            wrapper.Disconnect();
                }
            }

        }
        // Timer adapter (Action<T,int>) so TryReview can be subscribed to the thread pool.
        private static void ReviewCallBack(SecuritySocket wrapper, int time)
        {
            TryReview(wrapper);
        }
        private bool TryDequeueSend(out byte[] buffer)
        {
            buffer = null;
            lock (locckss)
                if (SendQueue.Count != 0)
                    buffer = SendQueue.Dequeue();
            return buffer != null;
        }
        public static void TrySend(SecuritySocket wrapper)
        {
            if (!wrapper.Alive) return;
            byte[] buffer;

            while (wrapper.TryDequeueSend(out buffer))
            {
                try
                {
                    wrapper.Connection.Send(buffer);
                }
                catch
                {
                    wrapper.Disconnect();
                }
            }
        }
        public int tstgg = 0;
        object locckss = new object();
        public unsafe void Send(Packet msg)
        {
            try
            {
                if (!IsBot)
                {
                    if (Alive)
                    {
                        // [PacketLogger] server->client, plaintext (before encryption). Off unless Data\packetlog.txt exists.
                        if (PacketLogger.Enabled)
                            PacketLogger.Log(true, Game, msg.Memory, msg.Size);
                        PerfMetrics.PacketOut(msg.Size);
                        if (SetDHKey && msg.Size >= 4)
                        {
                            // QahrMobile: a packet the mobile client keeps as a C struct goes out as that struct
                            ushort sid = *(ushort*)(msg.Memory + 2);
                            ushort mid = Phone.MobileProtocol.MobileId(sid);
                            if (Phone.MobileProtocol.IsBlocked(sid))
                                return;// Data\mobile_block.txt (testing)
                            if (mid != sid && Phone.MobileProtocol.IsStruct(mid) && !Phone.MobileLayouts.Has(sid))
                            {
                                Phone.MobileProtocol.ReportDrop(sid, mid, "out");// struct on the mobile, no layout yet
                                return;
                            }
                            byte[] converted;
                            try
                            {
                                converted = Phone.MobileLayouts.Outgoing(msg, mid);
                            }
                            catch (InvalidOperationException)
                            {
                                // QahrMobile: the conversion does not fit Packet.MAX_SIZE (Seal guard): drop this packet
                                // instead of disconnecting the client.
                                Phone.MobileProtocol.ReportDrop(sid, mid, "out-too-big");
                                return;
                            }
                            if (converted != null && converted.Length > Packet.MAX_SIZE)
                            {
                                Phone.MobileProtocol.ReportDrop(sid, mid, "out-too-big");// QahrMobile: never overflow the buffer
                                return;
                            }
                            if (converted != null)
                            {
                                using (var rec = new RecycledPacket())
                                {
                                    var p = rec.GetStream();
                                    fixed (byte* b = converted)
                                        p.memcpy(p.Memory, b, converted.Length);
                                    p.Size = converted.Length;
                                    SendWire(p);
                                }
                                return;
                            }
                        }
                        // QahrMobile: server id -> the mobile client's id for this send only (the packet may go to others)
                        ushort serverId = SetDHKey ? Phone.MobileProtocol.ToMobile(msg) : (ushort)0;
                        try
                        {
                            SendWire(msg);
                        }
                        finally
                        {
                            Phone.MobileProtocol.Restore(msg, serverId);
                        }
                    }
                }
            }
            catch (Exception e)
            {
                Disconnect();
            }
        }
        /// <summary>Encrypt and send one ready packet (V2 queue or the legacy BeginSend).</summary>
        private unsafe void SendWire(Packet msg)
        {
            if (UseV2)
            {
                SendV2(msg, true);
                return;
            }
            lock (SendRoot)
            {
                byte[] _buffer = new byte[msg.Size];

                if (Crypto != null)
                    Crypto.Encrypt(msg.Memory, 0, _buffer, 0, msg.Size);
                else
                    fixed (byte* ptr = _buffer)
                        msg.memcpy(ptr, msg.Memory, msg.Size);

                Connection.BeginSend(_buffer, 0, _buffer.Length, SocketFlags.None, new AsyncCallback(EndSend), null);
            }
        }
        public unsafe void SendProtectionPacket(Packet msg)
        {
            try
            {
                if (!IsBot)
                {
                    if (Alive)
                    {
                        PerfMetrics.PacketOut(msg.Size);
                        if (UseV2)
                        {
                            SendV2(msg, false);
                            return;
                        }
                        lock (SendRoot)
                        {
                            byte[] _buffer = new byte[msg.Size];

                            fixed (byte* ptr = _buffer)
                                msg.memcpy(ptr, msg.Memory, msg.Size);

                            Connection.BeginSend(_buffer, 0, _buffer.Length, SocketFlags.None, new AsyncCallback(EndSend), null);
                        }
                    }
                }
            }
            catch (Exception e)
            {
                Disconnect();
            }
        }
        private void EndSend(IAsyncResult res)
        {
            try { Connection.EndSend(res); }
            catch { Disconnect(); }
        }
        public ReceiveBuffer DHKeyBuffer = new ReceiveBuffer(1024, true);
        public ReceiveBuffer EncryptedDHKeyBuffer = new ReceiveBuffer(1024, true);
        public bool IsCompleteDHKey(out int type)
        {

            type = 0;
            try
            {
                if (DHKeyBuffer.Length() < 8)
                    return false;

                byte[] buffer = new byte[Packet.SealSize];
                for (int x = 0; x < buffer.Length; x++)
                    buffer[x] = DHKeyBuffer.buffer[x + (DHKeyBuffer.Length() - Packet.SealSize)];

                string Text = System.Text.ASCIIEncoding.ASCII.GetString(DHKeyBuffer.buffer);

                bool accept = Text.Contains("TQClient");
                if (!Text.EndsWith("TQClient"))
                    type = 1;
                return accept;
            }
            catch (Exception e)
            {
                MyConsole.SaveException(e);
                Disconnect();
                return false;
            }
        }
        public unsafe bool ReceiveDHKey()
        {
            try
            {


                if (Alive && WindowsAPI.ws2_32.CanRead(this))
                {
                    if (!SetDHKey && Alive)
                    {
                        SocketError Socket_Error = SocketError.IsConnected;
                        int length = DHKeyBuffer.MaxLength() - DHKeyBuffer.Length();
                        if (length <= 0)
                        {
                            Disconnect();
                            return false;
                        }
                        //if (!SetAfterConnectKey)
                        //    length = 276;
                        int ret = Connection.Receive(DHKeyBuffer.buffer, DHKeyBuffer.Length(), length, SocketFlags.None, out Socket_Error);
                        if (ret > 0)
                        {
                            if (HandleDHKeyBytes(ret))
                                return true;
                        }
                        else if (ret <= 0 || Socket_Error == SocketError.ConnectionAborted)
                        {
                            Disconnect();
                            return false;
                        }

                    }
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e.ToString());
                Disconnect();
            }
            return false;
        }
        // The DH-handshake handling of `ret` new bytes that were just received into DHKeyBuffer at
        // DHKeyBuffer.Length(). Moved unchanged out of ReceiveDHKey() so the V2 engine runs exactly the same
        // steps (incl. the 2562 case that keeps DHKeyBuffer's length). Returns true only for the 2562 case.
        private unsafe bool HandleDHKeyBytes(int ret)
        {
            int rec_type = 0;
            System.Buffer.BlockCopy(DHKeyBuffer.buffer, DHKeyBuffer.Length(), EncryptedDHKeyBuffer.buffer, EncryptedDHKeyBuffer.Length(), ret);
            EncryptedDHKeyBuffer.AddLength(ret);
            var Packetid = BitConverter.ToUInt16(DHKeyBuffer.buffer, 2);
            if (Packetid == 2562)
            {
                SetAfterConnectKey = true;
                return true;
            }
            if (Crypto != null)
            {
                fixed (byte* ptr = DHKeyBuffer.buffer)
                    Crypto.Decrypt(DHKeyBuffer.buffer, DHKeyBuffer.Length(), ptr, DHKeyBuffer.Length(), ret);
            }
            DHKeyBuffer.AddLength(ret);
            if (IsCompleteDHKey(out rec_type))
            {

                using (var rec = new RecycledPacket())
                {
                    var stream = rec.GetStream();
                    stream.Seek(0);
                    fixed (byte* ptr = DHKeyBuffer.buffer)
                        stream.memcpy(stream.Memory, ptr, DHKeyBuffer.Length());

                    stream.Size = DHKeyBuffer.Length();
                    PerfMetrics.PacketIn(stream.Size);

                    if (OnReceiveHandler != null)
                        OnReceiveHandler.Invoke(this, stream);
                }
            }
            return false;
        }
        public bool ReceiveBuffer()
        {
            if (Alive)
            {
                LastReceiveCall = Time32.Now;
                {
                    try
                    {
                        if (!ConnectFull)
                            return false;
                        if (!SetDHKey && Crypto != null)
                        {
                            ReceiveDHKey();
                        }
                        else if (WindowsAPI.ws2_32.CanRead(this))
                        {
                            try
                            {
                                if (!Alive)
                                    return false;
                                int available = Connection.Available;
                                if (available == 0)
                                {
                                    Disconnect();
                                    return false;
                                }
                                if (!Receive(available))
                                {
                                    return false;
                                }

                            }
                            catch (Exception e)
                            {
                                MyConsole.SaveException(e);
                                Disconnect();
                            }
                            return true;
                        }


                    }
                    catch (Exception e) { Disconnect(); Console.WriteLine(e.ToString()); }
                }
            }
            return false;
        }

        public unsafe bool HandlerBuffer()
        {
            HandlerBufferCore(30);
            return false;
        }

        // Frames and dispatches up to maxPackets complete packets from ReceiveBuff (Legacy: 30 per poll).
        // Returns true when it stopped because maxPackets was reached (there may be more complete packets).
        private unsafe bool HandlerBufferCore(int maxPackets)
        {
            int counts = maxPackets;
            while (true && counts > 0)
            {
                counts--;
                if (!Alive)
                    return false;
                try
                {

                    if (!ConnectFull)
                        return false;
                    if (ReceiveBuff.Length() == 0)
                        return false;
                    int Length = (int)(ReceiveBuff.ReadHead() + (IsGameServer ? 8 : 0));
                    if (Program.ServerConfig.IsInterServer || OnInterServer)
                        Length += 8;

                    if (Length < 2)
                        return false;
                    if (Length > ServerSockets.ReceiveBuffer.HeadSize)
                    {
                        Disconnect();
                        return false;
                    }
                    if (Length > ReceiveBuff.Length())
                        return false;

                    LastReceive = Time32.Now;
                    Packet Stream = PacketRecycle.Take();

                    Stream.Seek(0);

                    fixed (byte* ptr = ReceiveBuff.buffer)
                    {
                        Stream.memcpy(Stream.stream, ptr, Length);
                        if (Length < ReceiveBuff.Length())
                        {
                            fixed (void* next_buffer = &ReceiveBuff.buffer[Length])
                            {
                                Stream.memcpy(ptr, next_buffer, ReceiveBuff.Length() - Length);
                            }
                        }
                        Stream.Size = Length;

                        ReceiveBuff.DelLength(Length);
                    }

                    Stream.SeekForward(2);
                    PerfMetrics.PacketIn(Length);

                    if (OnReceiveHandler != null)
                        OnReceiveHandler.Invoke(this, Stream);

                }
                catch (Exception e) { Console.WriteLine(e.ToString()); }
            }
            return true;
        }

        public bool Receive(int available)
        {
            if (Alive)
            {
                SocketError Socket_Error = SocketError.IsConnected;
                try
                {
                    int length = ReceiveBuff.MaxLength() - ReceiveBuff.Length();

                    int receive_size = 0;
                    if (available > length)
                        receive_size = length;
                    else
                        receive_size = available;
                    if (receive_size == 0)
                    {
                        return false;
                    }
                    int ret = Connection.Receive(ReceiveBuff.buffer, ReceiveBuff.Length(), receive_size, SocketFlags.None, out Socket_Error);

                    if (ret > 0)
                    {

                        if (Crypto != null)
                        {
                            fixed (byte* ptr = ReceiveBuff.buffer)
                                Crypto.Decrypt(ReceiveBuff.buffer, ReceiveBuff.Length(), ptr, ReceiveBuff.Length(), ret);

                            //if (Program.packetsForm != null)
                            //{
                            //    Program.packetsForm.AddPacketToClient(ReceiveBuff.buffer);
                            //}
                        }
                        ReceiveBuff.AddLength(ret);

                        return true;
                    }
                    else if (ret == 0)//<= 0 || Socket_Error == SocketError.ConnectionAborted) 
                    {
                        Disconnect();
                        return false;
                    }
                    else//ret < 0 
                    {
                        if (Socket_Error != SocketError.WouldBlock)
                            Disconnect();
                    }


                }
                catch (Exception e)
                {
                    Console.WriteLine(e.ToString());
                    Console.WriteLine(Socket_Error.ToString());
                    // Unexpected receive failure: disconnect instead of leaving the socket Alive,
                    // which would spin the 1ms receive timer hot on a repeatedly-throwing socket.
                    Disconnect();
                }
            }
            return false;
        }



        public int nPutBytes = 0;
        public byte[] packet = null;

        public object SynRoot = new object();
        public void Disconnect()
        {
            lock (SynRoot)
            {
                if (Alive)
                {
                    Alive = false;
                    // Release this client's DoS connection count exactly once (balances DosProtection.Check
                    // on accept). Capture the IP before the socket is closed; DosCounted ensures only
                    // accepted/counted sockets decrement, and the Alive guard ensures it happens once.
                    if (DosCounted)
                    {
                        DosCounted = false;
                        string dosIp = RemoteIp;
                        if (!string.IsNullOrEmpty(dosIp) && dosIp != "NONE")
                            Qahr.Mobile.Base.Security.DosProtection.DisconnectClient(dosIp);
                        else
                            Qahr.Mobile.Base.Security.DosProtection.DecrementTotalConnections();
                    }
                    if (TimerSubscriptions != null)
                    {
                        for (int i = 0; i < TimerSubscriptions.Length; i++)
                            if (TimerSubscriptions[i] != null)
                                TimerSubscriptions[i].Dispose();
                    }
                    try
                    {
                        // [perf N1c] Was shutdown + closesocket(Handle) + Dispose(): the handle was closed
                        // twice (Dispose closes the SafeHandle again), which could close another player's
                        // freshly accepted socket that reused the handle value. Shutdown + Dispose only.
                        SocketHelpers.ShutdownAndDispose(Connection);
                        GC.SuppressFinalize(Connection);
                    }
                    catch (Exception e)
                    {
                        MyConsole.SaveException(e);
                    }
                    finally
                    {
                        if (UseV2)
                            OnClosedV2();
                        if (OnDisconnect != null)
                            OnDisconnect.Invoke(this);
                    }
                }
            }
        }
        public string GetMACAddress()
        {
            NetworkInterface[] nics = NetworkInterface.GetAllNetworkInterfaces();
            String sMacAddress = string.Empty;
            foreach (NetworkInterface adapter in nics)
            {
                if (sMacAddress == String.Empty)// only return MAC Address from first card  
                {
                    IPInterfaceProperties properties = adapter.GetIPProperties();
                    sMacAddress = adapter.GetPhysicalAddress().ToString();
                }
            }
            return sMacAddress;
        }


    }
}