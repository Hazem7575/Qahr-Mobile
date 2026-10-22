using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;

namespace Qahr.Mobile
{
    /// <summary>
    /// Developer-only packet INJECTOR: lets a local tool (PacketStudio's "Test" page) ask the server to send a raw
    /// plaintext packet to one online player's client, through the normal encrypted send path (GameClient.Send, which
    /// writes the TQServer seal). This is how you test "what packet opens window X" live, without a rebuild/restart.
    ///
    /// It never bypasses encryption and never talks to the client directly: the SERVER sends, exactly as it would for
    /// any real packet, so the injected packet also shows up in the live stream (S->C) as confirmation.
    ///
    /// SAFETY (why this is not a free-for-all):
    ///   * listens on 127.0.0.1 ONLY and requires a shared secret (>= 8 chars);
    ///   * opt-in via a file that the owner creates;
    ///   * an ALLOWLIST: a send is refused unless the target player's name or UID is listed in "allow=". With no
    ///     allowlist, every send is refused. So even if the secret leaks, injection can only ever hit the test
    ///     characters the owner explicitly listed, never real players.
    /// Injecting a malformed packet can disconnect or glitch the target client, so list TEST accounts only.
    ///
    /// ON:  create Infinity[DB]\packetinject.txt  with  secret=...(>=8)  allow=TestChar,12345  [port=17958]
    /// OFF: delete the file. Re-checked every ~3 s with packetlog.txt / packetstream.txt. Needs one restart after the
    ///      build that added this file.
    ///
    /// WIRE PROTOCOL v1 (little-endian), one connection carries many commands:
    ///   tool -> server (handshake): "PINJ" u16 secretLen, secret UTF-8   (within 5 s)
    ///   server -> tool:             "PIOK" u32 version(=1)   or   "PINO" and close
    ///   tool -> server command:  u32 bodyLen, body[bodyLen], body[0] = cmd
    ///     cmd 1 SEND:   u8 targetKind(0=uid,1=name),
    ///                   if uid:  u32 uid
    ///                   if name: u16 nameLen, name UTF-8
    ///                   then     u16 pktLen, packet[pktLen]   (plaintext, incl 4-byte header, excl 8-byte seal; <= 1024)
    ///     cmd 2 LIST:   (no args) -> server replies with the online players and whether each is allowed
    ///   server -> tool reply:    u32 bodyLen, body[bodyLen], body[0] = kind
    ///     kind 1 ACK:   u8 status(0 ok,1 offline,2 badPacket,3 tooBig,4 error,5 notAllowed), u32 uid, u16 msgLen, msg UTF-8
    ///     kind 2 LIST:  u16 count, count x { u32 uid, u8 allowed, u16 nameLen, name UTF-8 }
    /// </summary>
    public static class PacketInject
    {
        public const uint Version = 1;
        private const int MaxClients = 2;

        private static readonly object _gate = new object();
        private static TcpListener _listener;
        private static int _port;
        private static string _secret;
        private static string _lastError;
        private static volatile bool _configured;
        private static volatile int _clientCount;

        // allowlist (targets injection may ever hit)
        private static volatile HashSet<uint> _allowUid = new HashSet<uint>();
        private static volatile HashSet<string> _allowName = new HashSet<string>(StringComparer.OrdinalIgnoreCase);

        public static bool Listening { get { return _listener != null; } }
        public static int ClientCount { get { return _clientCount; } }

        /// <summary>Called every ~3 s from PacketLogger.Refresh. Never throws.</summary>
        internal static void Refresh()
        {
            try
            {
                // Production lockdown: the injector is a developer test tool. It only runs in development mode.
                if (!ServerMode.IsDevelopment)
                {
                    if (_configured)
                        Shutdown();
                    return;
                }
                string cfg = Program.ServerConfig.DbLocation + "packetinject.txt";
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

        public static void Configure(string[] lines)
        {
            try
            {
                int port = 17958;
                string secret = null;
                var allowUid = new HashSet<uint>();
                var allowName = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
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
                        case "allow":
                            foreach (var p in val.Split(new[] { ',', ' ', ';' }, StringSplitOptions.RemoveEmptyEntries))
                            {
                                uint u;
                                if (uint.TryParse(p, out u)) allowUid.Add(u);
                                else allowName.Add(p);
                            }
                            break;
                    }
                }
                _allowUid = allowUid; _allowName = allowName;

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
                    _secret = secret;
                    _port = port;
                    _configured = true;
                    try
                    {
                        var l = new TcpListener(IPAddress.Loopback, port);
                        l.ExclusiveAddressUse = true;
                        l.Start(4);
                        _listener = l;
                        _lastError = null;
                        var t = new Thread(delegate () { AcceptLoop(l); }) { IsBackground = true, Name = "PacketInject.Accept" };
                        t.Start();
                        try { Console.WriteLine("[PacketInject] listening on 127.0.0.1:" + port + " (allow: " + (_allowUid.Count + _allowName.Count) + " targets)"); } catch { }
                    }
                    catch (Exception e)
                    {
                        StopListener();
                        string err = port + ":" + e.Message;
                        if (err != _lastError)
                        {
                            _lastError = err;
                            try { Console.WriteLine("[PacketInject] cannot listen on 127.0.0.1:" + port + " : " + e.Message); } catch { }
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
                _secret = null;
                _lastError = null;
            }
        }

        private static void StopListener()
        {
            try { if (_listener != null) _listener.Stop(); } catch { }
            _listener = null;
        }

        private static void AcceptLoop(TcpListener l)
        {
            while (true)
            {
                Socket s;
                try { s = l.AcceptSocket(); }
                catch { return; }
                try
                {
                    var t = new Thread(delegate () { Session(l, s); }) { IsBackground = true, Name = "PacketInject.Client" };
                    t.Start();
                }
                catch { try { s.Close(); } catch { } }
            }
        }

        private static readonly byte[] Deny = { (byte)'P', (byte)'I', (byte)'N', (byte)'O' };

        private static void Session(TcpListener owner, Socket s)
        {
            bool counted = false;
            try
            {
                s.NoDelay = true;
                s.ReceiveTimeout = 5000;
                s.SendTimeout = 10000;
                byte[] head = ReadExact(s, 6);
                bool ok = head != null && head[0] == 'P' && head[1] == 'I' && head[2] == 'N' && head[3] == 'J';
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
                    if (!ok || _listener != owner || _clientCount >= MaxClients)
                    {
                        try { s.Send(Deny); } catch { }
                        s.Close();
                        return;
                    }
                    var reply = new byte[8];
                    reply[0] = (byte)'P'; reply[1] = (byte)'I'; reply[2] = (byte)'O'; reply[3] = (byte)'K';
                    WriteU32(reply, 4, Version);
                    s.Send(reply);
                    _clientCount++;
                    counted = true;
                }
                s.ReceiveTimeout = 0;
                Loop(s);
            }
            catch { }
            finally
            {
                if (counted) lock (_gate) { _clientCount--; }
                try { s.Close(); } catch { }
            }
        }

        private static void Loop(Socket s)
        {
            while (true)
            {
                byte[] lenb = ReadExact(s, 4);
                if (lenb == null)
                    return;
                int bodyLen = (int)ReadU32(lenb, 0);
                if (bodyLen <= 0 || bodyLen > 1 << 20)
                    return;
                byte[] body = ReadExact(s, bodyLen);
                if (body == null)
                    return;
                try { Handle(s, body); }
                catch (Exception e) { Ack(s, 4, 0, "error: " + e.Message); }
            }
        }

        private static void Handle(Socket s, byte[] body)
        {
            byte cmd = body[0];
            if (cmd == 2) { SendList(s); return; }
            if (cmd != 1) { Ack(s, 4, 0, "unknown command " + cmd); return; }
            // second layer on top of Refresh(): never inject while in production mode
            if (!ServerMode.IsDevelopment) { Ack(s, 5, 0, "server is in production mode (test tools are locked)"); return; }

            int p = 1;
            byte targetKind = body[p++];
            uint uid = 0;
            string name = null;
            if (targetKind == 0)
            {
                uid = ReadU32(body, p); p += 4;
            }
            else
            {
                int nl = body[p] | (body[p + 1] << 8); p += 2;
                name = Encoding.UTF8.GetString(body, p, nl); p += nl;
            }
            int pktLen = body[p] | (body[p + 1] << 8); p += 2;
            if (pktLen < 4) { Ack(s, 2, uid, "packet too short"); return; }
            if (pktLen > 1024) { Ack(s, 3, uid, "packet too big (" + pktLen + " > 1024)"); return; }
            var pkt = new byte[pktLen];
            Buffer.BlockCopy(body, p, pkt, 0, pktLen);

            uint resolvedUid;
            string resolvedName;
            Client.GameClient client = FindClient(uid, name, out resolvedUid, out resolvedName);
            if (client == null) { Ack(s, 1, uid, "player is not online"); return; }

            // ALLOWLIST gate: only ever inject into explicitly listed test targets.
            if (!IsAllowed(resolvedUid, resolvedName))
            {
                Ack(s, 5, resolvedUid, "target '" + (resolvedName ?? resolvedUid.ToString()) + "' is not in allow= (add it to packetinject.txt)");
                return;
            }

            // GameClient.Send expects [packet bytes][8 bytes for the seal]; it writes "TQServer" at the end and keeps
            // the u16 length header. Force the header to the real packet length so the seal offset lines up.
            var buffer = new byte[pktLen + 8];
            Buffer.BlockCopy(pkt, 0, buffer, 0, pktLen);
            buffer[0] = (byte)pktLen; buffer[1] = (byte)(pktLen >> 8);
            try
            {
                client.Send(buffer);
                ushort id = (ushort)(pkt[2] | (pkt[3] << 8));
                Ack(s, 0, resolvedUid, "sent id " + id + " len " + pktLen + " to " + (resolvedName ?? resolvedUid.ToString()));
            }
            catch (Exception e) { Ack(s, 4, resolvedUid, "send failed: " + e.Message); }
        }

        private static bool IsAllowed(uint uid, string name)
        {
            var au = _allowUid; var an = _allowName;
            if (uid != 0 && au.Contains(uid)) return true;
            if (name != null && an.Contains(name)) return true;
            return false;
        }

        private static Client.GameClient FindClient(uint uid, string name, out uint resolvedUid, out string resolvedName)
        {
            resolvedUid = uid; resolvedName = name;
            try
            {
                if (name == null)
                {
                    Client.GameClient c;
                    if (Pool.GamePoll.TryGetValue(uid, out c) && c != null && c.Player != null)
                    {
                        try { resolvedName = c.Player.Name; } catch { }
                        return c;
                    }
                    return null;
                }
                foreach (var c in Pool.GamePoll.Values)
                {
                    try
                    {
                        if (c != null && c.Player != null && c.Player.Name != null
                            && string.Equals(c.Player.Name, name, StringComparison.OrdinalIgnoreCase))
                        {
                            resolvedUid = c.Player.UID;
                            resolvedName = c.Player.Name;
                            return c;
                        }
                    }
                    catch { }
                }
            }
            catch { }
            return null;
        }

        private static void SendList(Socket s)
        {
            var items = new List<KeyValuePair<uint, string>>();
            try
            {
                foreach (var c in Pool.GamePoll.Values)
                {
                    try
                    {
                        if (c != null && c.Player != null && c.Player.Name != null)
                            items.Add(new KeyValuePair<uint, string>(c.Player.UID, c.Player.Name));
                    }
                    catch { }
                }
            }
            catch { }

            using (var ms = new MemoryStream())
            {
                ms.WriteByte(2); // kind LIST
                int count = Math.Min(items.Count, 65535);
                ms.WriteByte((byte)count); ms.WriteByte((byte)(count >> 8));
                for (int i = 0; i < count; i++)
                {
                    var nb = Encoding.UTF8.GetBytes(items[i].Value);
                    int nl = Math.Min(nb.Length, 255);
                    var u = new byte[4]; WriteU32(u, 0, items[i].Key); ms.Write(u, 0, 4);
                    ms.WriteByte(IsAllowed(items[i].Key, items[i].Value) ? (byte)1 : (byte)0);
                    ms.WriteByte((byte)nl); ms.WriteByte((byte)(nl >> 8));
                    ms.Write(nb, 0, nl);
                }
                WriteFrame(s, ms.ToArray());
            }
        }

        private static void Ack(Socket s, byte status, uint uid, string msg)
        {
            try
            {
                var mb = Encoding.UTF8.GetBytes(msg ?? "");
                int ml = Math.Min(mb.Length, 4000);
                var body = new byte[1 + 1 + 4 + 2 + ml];
                body[0] = 1; // kind ACK
                body[1] = status;
                WriteU32(body, 2, uid);
                body[6] = (byte)ml; body[7] = (byte)(ml >> 8);
                Buffer.BlockCopy(mb, 0, body, 8, ml);
                WriteFrame(s, body);
            }
            catch { }
        }

        private static void WriteFrame(Socket s, byte[] body)
        {
            var frame = new byte[4 + body.Length];
            WriteU32(frame, 0, (uint)body.Length);
            Buffer.BlockCopy(body, 0, frame, 4, body.Length);
            int off = 0;
            while (off < frame.Length)
            {
                int w = s.Send(frame, off, frame.Length - off, SocketFlags.None);
                if (w <= 0) throw new IOException("send failed");
                off += w;
            }
        }

        private static byte[] ReadExact(Socket s, int n)
        {
            var buf = new byte[n];
            int got = 0;
            while (got < n)
            {
                int r;
                try { r = s.Receive(buf, got, n - got, SocketFlags.None); }
                catch { return null; }
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

        private static uint ReadU32(byte[] b, int o)
        {
            return (uint)(b[o] | (b[o + 1] << 8) | (b[o + 2] << 16) | (b[o + 3] << 24));
        }

        private static void WriteU32(byte[] b, int o, uint v)
        {
            b[o] = (byte)v; b[o + 1] = (byte)(v >> 8); b[o + 2] = (byte)(v >> 16); b[o + 3] = (byte)(v >> 24);
        }
    }
}
