using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Reflection;
using System.Text;
using System.Threading;

namespace Qahr.Mobile
{
    /// <summary>
    /// Plaintext packet logger between the game client and this GameServer (both directions).
    /// C->S packets are logged in Program.Game_Receive (already decrypted); S->C packets are
    /// logged in SecuritySocket.Send (before encryption), so no cipher work is needed.
    ///
    /// Toggle at RUNTIME (no rebuild/restart): create "packetlog.txt" in the DB folder
    /// (Program.ServerConfig.DbLocation, i.e. Infinity[DB]\). Delete it to stop. Re-checked every 3s.
    /// Optional lines in packetlog.txt:
    ///   ids=1009,2226      only these packet ids (empty/missing = all)
    ///   skip=2036,2020     never these ids (default skips MsgPing 2036 and MsgTick 2020)
    ///   player=Name        only this player (case-insensitive)
    ///   hex=96             hex bytes per packet (0 = none, -1 = whole packet)
    ///   dir=both|in|out    in = client->server, out = server->client
    /// Output: [exe folder]\PacketLogs\yyyy-MM-dd.log
    ///
    /// The same two hook points also feed <see cref="PacketStream"/> (live binary stream to a
    /// local tool such as PacketStudio), enabled by "packetstream.txt" in the same folder.
    /// </summary>
    public static unsafe class PacketLogger
    {
        private static readonly object _lock = new object();
        private static volatile bool _enabled;
        private static int _nextCheck = Environment.TickCount;
        private static bool _checkedOnce;
        private static int _refreshing;
        private static HashSet<ushort> _only = new HashSet<ushort>();
        private static HashSet<ushort> _skip = new HashSet<ushort>();
        private static string _player;
        private static int _hex = 96;
        private static bool _logIn = true, _logOut = true;
        private static StreamWriter _writer;
        private static string _writerDay;
        private static Dictionary<ushort, string> _names;

        /// <summary>True when the text log is on OR a PacketStudio client is connected to the stream.</summary>
        public static bool Enabled
        {
            get { Refresh(); return _enabled || PacketStream.Active; }
        }

        private static void Refresh()
        {
            int now = Environment.TickCount;
            if (_checkedOnce && now - _nextCheck < 0)
                return;
            // only one thread does the file-system check; the others keep the previous state
            if (Interlocked.CompareExchange(ref _refreshing, 1, 0) != 0)
                return;
            try
            {
                _checkedOnce = true;
                _nextCheck = now + 3000;
                RefreshLog();
                PacketStream.Refresh();
                PacketInject.Refresh();
            }
            catch { }
            finally
            {
                Interlocked.Exchange(ref _refreshing, 0);
            }
        }

        private static void RefreshLog()
        {
            try
            {
                string cfg = Program.ServerConfig.DbLocation + "packetlog.txt";
                if (!File.Exists(cfg))
                {
                    if (_enabled)
                    {
                        _enabled = false;
                        CloseWriter();
                    }
                    return;
                }
                var only = new HashSet<ushort>();
                var skip = new HashSet<ushort> { 2036, 2020 };
                string player = null;
                int hex = 96;
                bool logIn = true, logOut = true;
                foreach (var raw in File.ReadAllLines(cfg))
                {
                    string line = raw.Trim();
                    int eq = line.IndexOf('=');
                    if (line.Length == 0 || line.StartsWith("#") || eq < 0)
                        continue;
                    string key = line.Substring(0, eq).Trim().ToLowerInvariant();
                    string val = line.Substring(eq + 1).Trim();
                    switch (key)
                    {
                        case "ids": ParseIds(val, only); break;
                        case "skip": skip.Clear(); ParseIds(val, skip); break;
                        case "player": player = val.Length == 0 ? null : val; break;
                        case "hex": int.TryParse(val, out hex); break;
                        case "dir":
                            string d = val.ToLowerInvariant();
                            logIn = d != "out";
                            logOut = d != "in";
                            break;
                    }
                }
                _only = only; _skip = skip; _player = player; _hex = hex;
                _logIn = logIn; _logOut = logOut;
                if (!_enabled)
                {
                    _enabled = true;
                    Write("---- packet logging started " + DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + " ----");
                }
            }
            catch
            {
                _enabled = false;
            }
        }

        internal static void ParseIds(string val, HashSet<ushort> into)
        {
            foreach (var p in val.Split(new[] { ',', ' ', ';' }, StringSplitOptions.RemoveEmptyEntries))
            {
                ushort id;
                if (ushort.TryParse(p, out id))
                    into.Add(id);
            }
        }

        private static string Name(ushort id)
        {
            if (_names == null)
            {
                var map = new Dictionary<ushort, string>();
                try
                {
                    foreach (var f in typeof(Game.GamePackets).GetFields(BindingFlags.Public | BindingFlags.Static))
                    {
                        if (f.FieldType == typeof(ushort))
                        {
                            ushort v = (ushort)f.GetValue(null);
                            if (!map.ContainsKey(v))
                                map[v] = f.Name;
                        }
                    }
                }
                catch { }
                _names = map;
            }
            string n;
            return _names.TryGetValue(id, out n) ? n : "?";
        }

        /// <param name="toClient">true = server->client, false = client->server</param>
        public static void Log(bool toClient, Client.GameClient client, byte* data, int size)
        {
            if (data == null || size < 4)
                return;
            Refresh();
            if (PacketStream.Active)
            {
                try { PacketStream.Push(toClient, client, data, size); }
                catch { }
            }
            if (!_enabled)
                return;
            if (toClient ? !_logOut : !_logIn)
                return;
            ushort id = *(ushort*)(data + 2);
            if (_skip.Contains(id))
                return;
            if (_only.Count > 0 && !_only.Contains(id))
                return;
            string who = "-";
            try
            {
                if (client != null && client.Player != null && client.Player.Name != null)
                    who = client.Player.Name;
            }
            catch { }
            if (_player != null && !string.Equals(who, _player, StringComparison.OrdinalIgnoreCase))
                return;

            var sb = new StringBuilder(80 + Math.Min(size, 256) * 3);
            sb.Append(DateTime.Now.ToString("HH:mm:ss.fff"))
              .Append(toClient ? " S->C " : " C->S ")
              .Append('[').Append(who).Append("] ")
              .Append(id).Append(' ').Append(Name(id))
              .Append(" len=").Append(size);
            int n = _hex < 0 ? size : Math.Min(_hex, size);
            if (n > 0)
            {
                sb.Append(" |");
                for (int i = 0; i < n; i++)
                    sb.Append(' ').Append(data[i].ToString("X2"));
                if (n < size)
                    sb.Append(" ...");
            }
            Write(sb.ToString());
        }

        public static void Log(bool toClient, Client.GameClient client, byte[] buffer)
        {
            if (buffer == null || !Enabled)
                return;
            fixed (byte* p = buffer)
                Log(toClient, client, p, buffer.Length);
        }

        private static void Write(string line)
        {
            lock (_lock)
            {
                try
                {
                    string day = DateTime.Now.ToString("yyyy-MM-dd");
                    if (_writer == null || _writerDay != day)
                    {
                        CloseWriter();
                        string dir = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "PacketLogs");
                        Directory.CreateDirectory(dir);
                        _writer = new StreamWriter(Path.Combine(dir, day + ".log"), true, Encoding.UTF8);
                        _writer.AutoFlush = true;
                        _writerDay = day;
                    }
                    _writer.WriteLine(line);
                }
                catch { }
            }
        }

        private static void CloseWriter()
        {
            lock (_lock)
            {
                try
                {
                    if (_writer != null)
                        _writer.Dispose();
                }
                catch { }
                _writer = null;
                _writerDay = null;
            }
        }
    }

    /// <summary>
    /// Live plaintext packet stream for PacketStudio (tools\PacketStudio). Opt-in, local only.
    ///
    /// ON:  create Infinity[DB]\packetstream.txt (see packetstream.sample.txt). OFF: delete it.
    ///      Re-checked every 3 s together with packetlog.txt. Listens on 127.0.0.1 only.
    ///      Lines: port=17957  secret=...(>= 8 chars, required)  player=Name  ids=1,2  skip=2036
    ///             dir=both|in|out  queue=50000 (frames buffered per client before drop-oldest)
    /// Cost: when the file is missing or no client is authenticated, the hooks only pay the
    ///       existing TickCount comparison. With a client: one byte[] copy + a short lock per packet.
    ///       A slow client never blocks the game: its bounded queue drops the oldest frames.
    ///
    /// WIRE PROTOCOL v1 (little-endian):
    ///   client -> server: "PSTR" u16 secretLen, secret UTF-8   (within 5 s of connecting)
    ///   server -> client: "PSOK" u32 version(=1)  then messages;  or "PSNO" and close.
    ///   message: u32 bodyLen, then body[bodyLen] whose first byte is the kind:
    ///     kind 1 PACKET: u64 seq, i64 utcTicks, u8 dir(0=C->S,1=S->C), u32 playerUid,
    ///                    u16 nameLen, name UTF-8, u16 len, bytes[len]
    ///                    (bytes = full plaintext packet incl. 4-byte header, excl. 8-byte TQ seal)
    ///     kind 2 STATS:  u64 droppedForThisClient, u64 lastSeq, u32 queued   (every ~2 s, keep-alive)
    ///   seq is global across all streamed packets, so a gap means frames dropped for this client.
    /// </summary>
    public static unsafe class PacketStream
    {
        public const uint Version = 1;
        public const byte KindPacket = 1, KindStats = 2;
        private const int MaxClients = 4;

        private static volatile bool _active;              // listening AND at least one authenticated client
        private static volatile StreamClient[] _clients = new StreamClient[0];
        private static readonly object _gate = new object();
        private static TcpListener _listener;
        private static int _port;
        private static string _secret;
        private static string _lastError;
        private static long _seq;
        private static volatile bool _configured;

        private static volatile HashSet<ushort> _only = new HashSet<ushort>();
        private static volatile HashSet<ushort> _skip = new HashSet<ushort>();
        private static volatile string _player;
        private static volatile bool _in = true, _out = true;
        private static int _queueCap = 50000;

        public static bool Active { get { return _active; } }
        public static long LastSeq { get { return Interlocked.Read(ref _seq); } }
        public static int ClientCount { get { return _clients.Length; } }
        public static bool Listening { get { return _listener != null; } }

        /// <summary>Called every ~3 s (from PacketLogger.Refresh). Never throws.</summary>
        internal static void Refresh()
        {
            try
            {
                string cfg = Program.ServerConfig.DbLocation + "packetstream.txt";
                if (!File.Exists(cfg))
                {
                    if (_configured)
                        Shutdown();
                    return;
                }
                Configure(File.ReadAllLines(cfg));
            }
            catch { }
        }

        /// <summary>Applies config lines (also used directly by the test harness). Never throws.</summary>
        public static void Configure(string[] lines)
        {
            try
            {
                int port = 17957, queue = 50000;
                string secret = null, player = null;
                var only = new HashSet<ushort>();
                var skip = new HashSet<ushort>();
                bool din = true, dout = true;
                foreach (var raw in lines)
                {
                    string line = raw.Trim();
                    int eq = line.IndexOf('=');
                    if (line.Length == 0 || line.StartsWith("#") || eq < 0)
                        continue;
                    string key = line.Substring(0, eq).Trim().ToLowerInvariant();
                    string val = line.Substring(eq + 1).Trim();
                    switch (key)
                    {
                        case "port": int.TryParse(val, out port); break;
                        case "secret": secret = val; break;
                        case "player": player = val.Length == 0 ? null : val; break;
                        case "ids": PacketLogger.ParseIds(val, only); break;
                        case "skip": PacketLogger.ParseIds(val, skip); break;
                        case "queue": int.TryParse(val, out queue); break;
                        case "dir":
                            string d = val.ToLowerInvariant();
                            din = d != "out";
                            dout = d != "in";
                            break;
                    }
                }
                _only = only; _skip = skip; _player = player; _in = din; _out = dout;
                _queueCap = Math.Max(1000, Math.Min(queue, 1000000));

                if (secret == null || secret.Length < 8 || port <= 0 || port > 65535)
                {
                    if (_configured)
                        Shutdown();
                    return;
                }
                lock (_gate)
                {
                    if (_listener != null && port == _port && secret == _secret)
                        return;
                    StopListener();
                    DropAllClients();
                    _secret = secret;
                    _port = port;
                    _configured = true;
                    try
                    {
                        var l = new TcpListener(IPAddress.Loopback, port);
                        l.ExclusiveAddressUse = true;
                        l.Start(8);
                        _listener = l;
                        _lastError = null;
                        var t = new Thread(() => AcceptLoop(l)) { IsBackground = true, Name = "PacketStream.Accept" };
                        t.Start();
                        try { Console.WriteLine("[PacketStream] listening on 127.0.0.1:" + port); } catch { }
                    }
                    catch (Exception e)
                    {
                        StopListener();
                        string err = port + ":" + e.Message;
                        if (err != _lastError)
                        {
                            _lastError = err;
                            try { Console.WriteLine("[PacketStream] cannot listen on 127.0.0.1:" + port + " : " + e.Message); } catch { }
                        }
                    }
                }
            }
            catch { }
        }

        public static void Shutdown()
        {
            lock (_gate)
            {
                _configured = false;
                StopListener();
                DropAllClients();
                _secret = null;
                _lastError = null;
            }
        }

        private static void StopListener()
        {
            try { if (_listener != null) _listener.Stop(); } catch { }
            _listener = null;
        }

        private static void DropAllClients()
        {
            var list = _clients;
            _clients = new StreamClient[0];
            _active = false;
            foreach (var c in list)
                c.Close();
        }

        private static void AcceptLoop(TcpListener l)
        {
            while (true)
            {
                Socket s;
                try { s = l.AcceptSocket(); }
                catch { return; } // listener stopped
                try
                {
                    var t = new Thread(() => Session(l, s)) { IsBackground = true, Name = "PacketStream.Client" };
                    t.Start();
                }
                catch { try { s.Close(); } catch { } }
            }
        }

        private static readonly byte[] Deny = { (byte)'P', (byte)'S', (byte)'N', (byte)'O' };

        private static void Session(TcpListener owner, Socket s)
        {
            StreamClient c = null;
            try
            {
                s.NoDelay = true;
                s.ReceiveTimeout = 5000;
                s.SendTimeout = 10000;
                byte[] head = ReadExact(s, 6);
                bool ok = head != null && head[0] == 'P' && head[1] == 'S' && head[2] == 'T' && head[3] == 'R';
                byte[] given = null;
                if (ok)
                {
                    int n = head[4] | (head[5] << 8);
                    ok = n > 0 && n <= 256;
                    if (ok)
                        given = ReadExact(s, n);
                }
                string secret = _secret;
                ok = ok && given != null && secret != null && FixedEquals(given, Encoding.UTF8.GetBytes(secret));
                lock (_gate)
                {
                    if (!ok || _listener != owner || _clients.Length >= MaxClients)
                    {
                        try { s.Send(Deny); } catch { }
                        s.Close();
                        return;
                    }
                    var reply = new byte[8];
                    reply[0] = (byte)'P'; reply[1] = (byte)'S'; reply[2] = (byte)'O'; reply[3] = (byte)'K';
                    WriteU32(reply, 4, Version);
                    s.Send(reply);
                    s.ReceiveTimeout = 0;
                    c = new StreamClient(s, _queueCap);
                    var arr = new StreamClient[_clients.Length + 1];
                    Array.Copy(_clients, arr, _clients.Length);
                    arr[arr.Length - 1] = c;
                    _clients = arr;
                    _active = true;
                }
                c.Run(); // returns when the client disconnects or is dropped
            }
            catch { }
            finally
            {
                if (c != null)
                    Remove(c);
                else
                    try { s.Close(); } catch { }
            }
        }

        private static void Remove(StreamClient c)
        {
            lock (_gate)
            {
                var list = new List<StreamClient>(_clients);
                if (list.Remove(c))
                {
                    _clients = list.ToArray();
                    _active = _clients.Length > 0;
                }
            }
            c.Close();
        }

        private static byte[] ReadExact(Socket s, int n)
        {
            var buf = new byte[n];
            int got = 0;
            while (got < n)
            {
                int r = s.Receive(buf, got, n - got, SocketFlags.None);
                if (r <= 0)
                    return null;
                got += r;
            }
            return buf;
        }

        private static bool FixedEquals(byte[] a, byte[] b)
        {
            if (b.Length == 0)
                return false;
            int diff = a.Length ^ b.Length;
            for (int i = 0; i < a.Length; i++)
                diff |= a[i] ^ b[i % b.Length];
            return diff == 0;
        }

        /// <summary>Hot path, called only when Active. Builds the frame once and queues it to every client.</summary>
        public static void Push(bool toClient, Client.GameClient client, byte* data, int size)
        {
            uint uid = 0;
            string name = null;
            try
            {
                if (client != null && client.Player != null)
                {
                    uid = client.Player.UID;
                    name = client.Player.Name;
                }
            }
            catch { }
            Push(toClient, uid, name, data, size);
        }

        public static void Push(bool toClient, uint uid, string name, byte* data, int size)
        {
            if (!_active || data == null || size < 4)
                return;
            if (toClient ? !_out : !_in)
                return;
            ushort id = *(ushort*)(data + 2);
            var skip = _skip;
            if (skip.Count > 0 && skip.Contains(id))
                return;
            var only = _only;
            if (only.Count > 0 && !only.Contains(id))
                return;
            string player = _player;
            if (player != null && !string.Equals(name, player, StringComparison.OrdinalIgnoreCase))
                return;

            // the header length excludes the 8-byte TQ seal; the buffers at the hooks include it
            int len = *(ushort*)data;
            if (len < 4 || len > size)
                len = size;
            if (len > 65535)
                len = 65535;

            byte[] nameBytes = string.IsNullOrEmpty(name) ? null : Encoding.UTF8.GetBytes(name);
            int nameLen = nameBytes == null ? 0 : Math.Min(nameBytes.Length, 255);
            int body = 1 + 8 + 8 + 1 + 4 + 2 + nameLen + 2 + len;
            var f = new byte[4 + body];
            WriteU32(f, 0, (uint)body);
            f[4] = KindPacket;
            long seq = Interlocked.Increment(ref _seq);
            WriteU64(f, 5, (ulong)seq);
            WriteU64(f, 13, (ulong)DateTime.UtcNow.Ticks);
            f[21] = (byte)(toClient ? 1 : 0);
            WriteU32(f, 22, uid);
            f[26] = (byte)nameLen; f[27] = 0;
            if (nameLen > 0)
                Buffer.BlockCopy(nameBytes, 0, f, 28, nameLen);
            int p = 28 + nameLen;
            f[p] = (byte)len; f[p + 1] = (byte)(len >> 8);
            p += 2;
            fixed (byte* dst = &f[p])
                Buffer.MemoryCopy(data, dst, len, len);

            var clients = _clients;
            for (int i = 0; i < clients.Length; i++)
                clients[i].Enqueue(f);
        }

        internal static void WriteU32(byte[] b, int o, uint v)
        {
            b[o] = (byte)v; b[o + 1] = (byte)(v >> 8); b[o + 2] = (byte)(v >> 16); b[o + 3] = (byte)(v >> 24);
        }

        internal static void WriteU64(byte[] b, int o, ulong v)
        {
            WriteU32(b, o, (uint)v);
            WriteU32(b, o + 4, (uint)(v >> 32));
        }

        private sealed class StreamClient
        {
            private readonly Socket _s;
            private readonly int _cap;
            private readonly Queue<byte[]> _q = new Queue<byte[]>();
            private readonly object _qlock = new object();
            private readonly AutoResetEvent _signal = new AutoResetEvent(false);
            private long _dropped;
            private volatile bool _closed;

            public StreamClient(Socket s, int cap) { _s = s; _cap = cap; }

            public void Enqueue(byte[] frame)
            {
                if (_closed)
                    return;
                bool wake;
                lock (_qlock)
                {
                    if (_q.Count >= _cap)
                    {
                        _q.Dequeue();
                        _dropped++;
                    }
                    _q.Enqueue(frame);
                    wake = _q.Count == 1;
                }
                if (wake)
                {
                    try { _signal.Set(); } catch { }
                }
            }

            public void Run()
            {
                var batch = new List<byte[]>(256);
                var buf = new byte[64 * 1024];
                var sink = new byte[256];
                int nextStats = Environment.TickCount + 2000;
                try
                {
                    SendStats();
                    while (!_closed)
                    {
                        _signal.WaitOne(500);
                        while (!_closed)
                        {
                            int queued;
                            lock (_qlock)
                            {
                                while (_q.Count > 0 && batch.Count < 256)
                                    batch.Add(_q.Dequeue());
                                queued = _q.Count;
                            }
                            if (batch.Count == 0)
                                break;
                            int used = 0;
                            foreach (var f in batch)
                            {
                                if (used + f.Length > buf.Length)
                                {
                                    SendAll(buf, used);
                                    used = 0;
                                }
                                if (f.Length > buf.Length)
                                    SendAll(f, f.Length);
                                else
                                {
                                    Buffer.BlockCopy(f, 0, buf, used, f.Length);
                                    used += f.Length;
                                }
                            }
                            if (used > 0)
                                SendAll(buf, used);
                            batch.Clear();
                            if (queued == 0)
                                break;
                        }
                        if (Environment.TickCount - nextStats >= 0)
                        {
                            nextStats = Environment.TickCount + 2000;
                            SendStats();
                        }
                        if (_s.Poll(0, SelectMode.SelectRead))
                        {
                            if (_s.Available == 0)
                                break; // peer closed
                            _s.Receive(sink, Math.Min(sink.Length, _s.Available), SocketFlags.None); // ignore input
                        }
                    }
                }
                catch { }
                _closed = true;
            }

            private void SendStats()
            {
                var f = new byte[4 + 1 + 8 + 8 + 4];
                WriteU32(f, 0, (uint)(f.Length - 4));
                f[4] = KindStats;
                int queued;
                long dropped;
                lock (_qlock) { queued = _q.Count; dropped = _dropped; }
                WriteU64(f, 5, (ulong)dropped);
                WriteU64(f, 13, (ulong)LastSeq);
                WriteU32(f, 21, (uint)queued);
                SendAll(f, f.Length);
            }

            private void SendAll(byte[] b, int n)
            {
                int off = 0;
                while (off < n)
                {
                    int w = _s.Send(b, off, n - off, SocketFlags.None);
                    if (w <= 0)
                        throw new IOException("send failed");
                    off += w;
                }
            }

            public void Close()
            {
                _closed = true;
                try { _signal.Set(); } catch { }
                try { _s.Shutdown(SocketShutdown.Both); } catch { }
                try { _s.Close(); } catch { }
                lock (_qlock) { _q.Clear(); }
            }
        }
    }
}
