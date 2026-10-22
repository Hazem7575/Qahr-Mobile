// ============================================================================
//  [net V2] Second network engine for SecuritySocket, behind a flag.
//
//  ServerConfig.ini:
//      [Network]
//      NetEngine=Legacy        ; Legacy (default) | V2
//      MaxSendQueueKB=4096     ; V2 only: per-connection cap of encrypted bytes waiting to be sent
//                              ;          (a client that can't keep up is disconnected). 0 = no cap.
//      SendBatchKB=64          ; V2 only: max bytes handed to one SendAsync (several packets, one syscall)
//
//  What V2 changes (only for sockets accepted by a ServerSocket whose Open() picked V2):
//   * Receive: one SocketAsyncEventArgs per connection, always one receive posted (IOCP). No 1ms
//     Thread4K/StaticPool poll timer, no select(). The received bytes go into the SAME buffers as Legacy
//     (DHKeyBuffer during the DH handshake, ReceiveBuff afterwards), are decrypted in place in arrival
//     order, and are handled by the SAME code (HandleDHKeyBytes / HandlerBufferCore) -> same framing,
//     same TQProtect/DH handshake, same OnReceiveHandler (Program.Game_Receive / CreateDHKey).
//     All complete packets are dispatched right away (Legacy: max 30 per poll, the rest waited for the
//     next incoming bytes). Handlers run on the .NET thread pool (Legacy: the 64 ReceivePool threads);
//     per-connection they are still strictly sequential (the next receive is posted after dispatch).
//   * Send: Send()/SendProtectionPacket() encrypt into a pooled buffer UNDER SendRoot and enqueue it
//     under the same lock, so cipher order == queue order == wire order (as Legacy). One SendAsync in
//     flight per connection; a buffer goes back to the pool only after its send completed (or failed),
//     never before. Partial sends are completed.
//   * Idle check: the same TryReview(), from one shared 5s timer for all V2 sockets.
//  Legacy sockets never run any code in this file except SocketHelpers.
// ============================================================================
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Net.Sockets;
using System.Threading;
using System.Buffers;

namespace Qahr.Mobile.ServerSockets
{
    internal static class SocketHelpers
    {
        /// <summary>Graceful close of a socket handle exactly once: Shutdown(Both) then Dispose.</summary>
        public static void ShutdownAndDispose(Socket socket)
        {
            if (socket == null)
                return;
            try { socket.Shutdown(SocketShutdown.Both); }
            catch { } // not connected / listener / already closed
            try { socket.Dispose(); }
            catch { }
        }
    }

    public unsafe partial class SecuritySocket
    {
        /// <summary>Set by ServerSocket.Handle_Accept before Create() when the listener runs NetEngine=V2.</summary>
        public bool UseV2 = false;

        private static int V2MaxSendQueueBytes = 4096 * 1024;
        private static int V2SendBatchBytes = 64 * 1024;
        private const int V2MaxSegmentsPerSend = 64;

        private static readonly ConcurrentDictionary<SecuritySocket, byte> V2Sockets = new ConcurrentDictionary<SecuritySocket, byte>();
        private static Timer V2ReviewTimer;
        private static readonly object V2TimerLock = new object();

        public static int V2ConnectionCount { get { return V2Sockets.Count; } }

        public static void LoadV2Settings()
        {
            try
            {
                int q = Qahr.Mobile.SharedConfig.Int("Network", "MaxSendQueueKB", 4096);
                V2MaxSendQueueBytes = q <= 0 ? 0 : q * 1024;
                int b = Qahr.Mobile.SharedConfig.Int("Network", "SendBatchKB", 64);
                if (b < 1) b = 1;
                if (b > 1024) b = 1024;
                V2SendBatchBytes = b * 1024;
            }
            catch { }
        }

        private struct SendChunk
        {
            public byte[] Buffer;
            public int Length;
        }

        // ---- send state (sendQueue / v2Sending / v2QueuedBytes are guarded by SendRoot) ----
        private Queue<SendChunk> sendQueueV2;
        private bool v2Sending;
        private int v2QueuedBytes;
        private SocketAsyncEventArgs v2SendArgs;
        // Owned by the single running send pump (v2Sending == true) only.
        private readonly List<SendChunk> v2Inflight = new List<SendChunk>();
        private int v2InflightTotal, v2InflightSent;

        // ---- receive state ----
        private SocketAsyncEventArgs v2RecvArgs;
        private bool v2RecvIntoDH;
        private int v2RecvStopped;

        private void CreateV2()
        {
            TimerSubscriptions = null;
            sendQueueV2 = new Queue<SendChunk>();
            v2Sending = false;
            v2QueuedBytes = 0;
            v2SendArgs = new SocketAsyncEventArgs();
            v2SendArgs.Completed += OnSendCompletedV2;
            V2Sockets.TryAdd(this, 0);
            EnsureV2ReviewTimer();
        }

        private static void EnsureV2ReviewTimer()
        {
            if (V2ReviewTimer != null)
                return;
            lock (V2TimerLock)
            {
                if (V2ReviewTimer == null)
                    V2ReviewTimer = new Timer(V2ReviewTick, null, 5000, 5000);
            }
        }

        private static void V2ReviewTick(object state)
        {
            foreach (var pair in V2Sockets)
            {
                try { TryReview(pair.Key); }
                catch (Exception e) { MyConsole.SaveException(e); }
            }
        }

        /// <summary>Called from Disconnect() (inside SynRoot, after Alive=false and the socket was closed).</summary>
        private void OnClosedV2()
        {
            byte dummy;
            V2Sockets.TryRemove(this, out dummy);
            if (SendRoot == null || sendQueueV2 == null)
                return;
            lock (SendRoot)
            {
                // Chunks still in the queue were never handed to the socket -> safe to return now.
                // The in-flight batch (if any) is returned by its completion (aborted by the close).
                while (sendQueueV2.Count > 0)
                    ReturnBufferV2(sendQueueV2.Dequeue().Buffer);
                v2QueuedBytes = 0;
            }
        }

        private static byte[] RentBufferV2(int size)
        {
            return ArrayPool<byte>.Shared.Rent(size);
        }

        private static void ReturnBufferV2(byte[] buffer)
        {
            if (buffer != null)
                ArrayPool<byte>.Shared.Return(buffer);
        }

        // =====================================================================
        //  Receive
        // =====================================================================

        /// <summary>Starts the async receive loop (once, after ProcessConnection and ConnectFull).</summary>
        public void StartReceiveV2()
        {
            if (!UseV2 || !Alive || v2RecvArgs != null)
                return;
            v2RecvArgs = new SocketAsyncEventArgs();
            v2RecvArgs.Completed += OnReceiveCompletedV2;
            ReceiveLoopV2(false);
        }

        private void OnReceiveCompletedV2(object sender, SocketAsyncEventArgs e)
        {
            ReceiveLoopV2(true);
        }

        // Runs until a receive is pending (returns; the completion re-enters with completed=true) or the
        // connection is done. Synchronous completions are handled in the loop (no recursion).
        private void ReceiveLoopV2(bool completed)
        {
            try
            {
                while (true)
                {
                    if (completed)
                    {
                        completed = false;
                        if (!ProcessReceiveV2(v2RecvArgs))
                        {
                            StopReceiveV2();
                            return;
                        }
                    }
                    if (!Alive)
                    {
                        StopReceiveV2();
                        return;
                    }
                    if (!SetDHKey && Crypto != null)
                    {
                        // DH handshake: same target as Legacy ReceiveDHKey().
                        int length = DHKeyBuffer.MaxLength() - DHKeyBuffer.Length();
                        if (length <= 0)
                        {
                            Disconnect();
                            StopReceiveV2();
                            return;
                        }
                        v2RecvIntoDH = true;
                        v2RecvArgs.SetBuffer(DHKeyBuffer.buffer, DHKeyBuffer.Length(), length);
                    }
                    else
                    {
                        int length = ReceiveBuff.MaxLength() - ReceiveBuff.Length();
                        if (length <= 0)
                        {
                            // Can't happen with valid traffic (a frame is <= HeadSize == buffer size and
                            // complete frames were dispatched); the stream can't be framed any more.
                            Disconnect();
                            StopReceiveV2();
                            return;
                        }
                        v2RecvIntoDH = false;
                        v2RecvArgs.SetBuffer(ReceiveBuff.buffer, ReceiveBuff.Length(), length);
                    }
                    if (Connection.ReceiveAsync(v2RecvArgs))
                        return; // pending -> OnReceiveCompletedV2
                    completed = true;
                }
            }
            catch (ObjectDisposedException)
            {
                Disconnect();
                StopReceiveV2();
            }
            catch (Exception e)
            {
                MyConsole.SaveException(e);
                Disconnect();
                StopReceiveV2();
            }
        }

        private void StopReceiveV2()
        {
            if (Interlocked.Exchange(ref v2RecvStopped, 1) != 0)
                return;
            // No receive is pending when we get here, so the args can be released.
            try { if (v2RecvArgs != null) v2RecvArgs.Dispose(); }
            catch { }
        }

        // Returns false when the loop must stop (connection closed / error).
        private unsafe bool ProcessReceiveV2(SocketAsyncEventArgs e)
        {
            if (!Alive)
                return false;
            int ret = e.BytesTransferred;
            if (e.SocketError != SocketError.Success || ret <= 0)
            {
                Disconnect(); // ret == 0: remote closed (Legacy: Receive() == 0 -> Disconnect)
                return false;
            }
            LastReceiveCall = Time32.Now;

            if (v2RecvIntoDH)
            {
                try
                {
                    HandleDHKeyBytes(ret);
                }
                catch (Exception ex)
                {
                    // Legacy ReceiveDHKey(): Console.WriteLine + Disconnect.
                    Console.WriteLine(ex.ToString());
                    Disconnect();
                    return false;
                }
                return Alive;
            }

            // Same as Legacy Receive(): decrypt only the new bytes, in place, in arrival order.
            if (Crypto != null)
            {
                fixed (byte* ptr = ReceiveBuff.buffer)
                    Crypto.Decrypt(ReceiveBuff.buffer, ReceiveBuff.Length(), ptr, ReceiveBuff.Length(), ret);
            }
            ReceiveBuff.AddLength(ret);

            // Dispatch every complete packet now (Legacy: 30 per poll).
            while (Alive)
            {
                int before = ReceiveBuff.Length();
                if (!HandlerBufferCore(256))
                    break;
                if (ReceiveBuff.Length() == before)
                    break; // no progress (should not happen) - don't spin
            }
            return Alive;
        }

        // =====================================================================
        //  Send
        // =====================================================================

        private unsafe void SendV2(Packet msg, bool encrypt)
        {
            int size = msg.Size;
            if (size <= 0)
                return;
            bool start = false, overflow = false;
            lock (SendRoot)
            {
                if (!Alive || sendQueueV2 == null)
                    return;
                byte[] buffer = RentBufferV2(size);
                // Encrypt under SendRoot, then enqueue under the same lock: cipher order == wire order.
                if (encrypt && Crypto != null)
                    Crypto.Encrypt(msg.Memory, 0, buffer, 0, size);
                else
                    fixed (byte* ptr = buffer)
                        msg.memcpy(ptr, msg.Memory, size);
                sendQueueV2.Enqueue(new SendChunk { Buffer = buffer, Length = size });
                v2QueuedBytes += size;
                if (V2MaxSendQueueBytes > 0 && v2QueuedBytes > V2MaxSendQueueBytes)
                    overflow = true;
                else if (!v2Sending)
                {
                    v2Sending = true;
                    start = true;
                }
            }
            if (overflow)
            {
                MyConsole.WriteLine("[Network] V2 send queue over " + (V2MaxSendQueueBytes / 1024) + " KB, disconnecting " + RemoteIp, ConsoleColor.Yellow);
                Disconnect(); // never called while holding SendRoot
                return;
            }
            if (start)
                PumpSendV2();
        }

        // Only one pump runs per connection (v2Sending). Sends batches until the queue is empty or a
        // SendAsync is pending (then OnSendCompletedV2 continues).
        private void PumpSendV2()
        {
            try
            {
                while (true)
                {
                    List<ArraySegment<byte>> segments;
                    lock (SendRoot)
                    {
                        if (!Alive || sendQueueV2.Count == 0)
                        {
                            v2Sending = false;
                            return;
                        }
                        segments = new List<ArraySegment<byte>>();
                        v2Inflight.Clear();
                        int total = 0;
                        while (sendQueueV2.Count > 0 && segments.Count < V2MaxSegmentsPerSend)
                        {
                            SendChunk next = sendQueueV2.Peek();
                            if (segments.Count > 0 && total + next.Length > V2SendBatchBytes)
                                break;
                            sendQueueV2.Dequeue();
                            v2QueuedBytes -= next.Length;
                            v2Inflight.Add(next);
                            segments.Add(new ArraySegment<byte>(next.Buffer, 0, next.Length));
                            total += next.Length;
                        }
                        v2InflightTotal = total;
                        v2InflightSent = 0;
                    }
                    v2SendArgs.BufferList = segments;
                    if (Connection.SendAsync(v2SendArgs))
                        return; // pending -> OnSendCompletedV2
                    if (!CompleteSendV2())
                        return; // pending again (partial) or failed
                }
            }
            catch (Exception)
            {
                FailSendV2();
            }
        }

        private void OnSendCompletedV2(object sender, SocketAsyncEventArgs e)
        {
            try
            {
                if (CompleteSendV2())
                    PumpSendV2();
            }
            catch (Exception)
            {
                FailSendV2();
            }
        }

        // Processes a finished SendAsync of the in-flight batch. Returns true when the whole batch is on the
        // wire (buffers returned, pump may continue); false when a remainder send is pending or it failed.
        private bool CompleteSendV2()
        {
            while (true)
            {
                if (v2SendArgs.SocketError != SocketError.Success || v2SendArgs.BytesTransferred <= 0)
                {
                    FailSendV2();
                    return false;
                }
                v2InflightSent += v2SendArgs.BytesTransferred;
                if (v2InflightSent >= v2InflightTotal)
                {
                    for (int i = 0; i < v2Inflight.Count; i++)
                        ReturnBufferV2(v2Inflight[i].Buffer); // completed -> the buffer is free again
                    v2Inflight.Clear();
                    return true;
                }
                // Partial send: send the rest of the same batch before anything else.
                v2SendArgs.BufferList = RemainderSegmentsV2(v2InflightSent);
                if (Connection.SendAsync(v2SendArgs))
                    return false;
            }
        }

        private List<ArraySegment<byte>> RemainderSegmentsV2(int skip)
        {
            var list = new List<ArraySegment<byte>>();
            for (int i = 0; i < v2Inflight.Count; i++)
            {
                SendChunk c = v2Inflight[i];
                if (skip >= c.Length)
                {
                    skip -= c.Length;
                    continue;
                }
                list.Add(new ArraySegment<byte>(c.Buffer, skip, c.Length - skip));
                skip = 0;
            }
            return list;
        }

        // The in-flight send failed or could not start: its buffers are no longer used by the socket.
        private void FailSendV2()
        {
            for (int i = 0; i < v2Inflight.Count; i++)
                ReturnBufferV2(v2Inflight[i].Buffer);
            v2Inflight.Clear();
            v2InflightTotal = v2InflightSent = 0;
            lock (SendRoot)
            {
                v2Sending = false;
            }
            Disconnect(); // drains the queue (OnClosedV2); no-op if already closed
        }
    }
}
