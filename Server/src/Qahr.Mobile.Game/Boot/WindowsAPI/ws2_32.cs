using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.InteropServices;
using System.Net.Sockets;

namespace Qahr.Mobile.WindowsAPI
{
    public class ws2_32
    {

        [DllImport("msvcrt.dll", EntryPoint = "_time64", CallingConvention = CallingConvention.Cdecl)]
        public static extern ulong GetUnixTime64(long timer = 0);

        public const int SOCKET_ERROR = -1;
        public const int INVALID_SOCKET = ~0;


        private static IntPtr[] SocketListToFileDescriptorSet(List<Socket> socketList)
        {
            if (socketList == null || socketList.Count == 0)
                return null;
            IntPtr[] array = new IntPtr[socketList.Count + 1];
            array[0] = (IntPtr)socketList.Count;
            for (int i = 0; i < socketList.Count; i++)
            {
                if (!(socketList[i] is Socket))
                    continue;
                array[i + 1] = ((Socket)socketList[i]).Handle;
            }
            return array;
        }
        private static void SelectFileDescriptor(List<Socket> socketList, IntPtr[] fileDescriptorSet)
        {
            if (socketList == null || socketList.Count == 0)
                return;
            if ((int)fileDescriptorSet[0] == 0)
            {
                socketList.Clear();
                return;
            }
            for (int i = 0; i < socketList.Count; i++)
            {
                Socket socket = socketList[i] as Socket;
                int num = 0;
                while (num < (int)fileDescriptorSet[0] && !(fileDescriptorSet[num + 1] == socket.Handle))
                {
                    num++;
                }
                if (num == (int)fileDescriptorSet[0])
                {
                    socketList.RemoveAt(i--);
                }
            }
        }
        public static int SocketSelect(int FD_SETSIZE, List<Socket> readsocket, List<Socket> writesockets, List<Socket> errorsockets, int stampms)
        {

            IntPtr[] read = SocketListToFileDescriptorSet(readsocket);
            IntPtr[] write = SocketListToFileDescriptorSet(readsocket);
            IntPtr[] error = SocketListToFileDescriptorSet(readsocket);
            TimeValue time = new TimeValue(stampms);

            int ret = select(FD_SETSIZE, read, write, error, ref time);

            SelectFileDescriptor(readsocket, read);
            SelectFileDescriptor(writesockets, write);
            SelectFileDescriptor(errorsockets, error);

            return ret;
        }



        public static bool CanRead(ServerSockets.SecuritySocket socket)
        {

#if !MAC

            IntPtr[] array = new IntPtr[2] { (IntPtr)1, socket.Connection.Handle };
            TimeValue time = new TimeValue(1000);//1000
            int err = select(0, array, null, null, ref time);

            if (err == 0) return false;
            if ((SocketError)err == SocketError.SocketError) return false;
            if ((int)array[0] == 0) return false;



            return array[1] == socket.Connection.Handle;
#else
            return socket.Socket.Poll(1000, SelectMode.SelectWrite);
#endif
        }

        public static bool CanWrite(ServerSockets.SecuritySocket socket)
        {
#if !MAC
            IntPtr[] array = new IntPtr[2] { (IntPtr)1, socket.Connection.Handle };
            TimeValue time = new TimeValue(1000);//1000
            int err = select(0, null, array, null, ref time);
            if (err == 0) return false;
            if ((SocketError)err == SocketError.SocketError) return false;
            if ((int)array[0] == 0) return false;
            return array[1] == socket.Connection.Handle;
#else
            return socket.Socket.Poll(1000, SelectMode.SelectWrite);
#endif
        }

        [StructLayout(LayoutKind.Sequential)]
        internal struct TimeValue
        {
            public int Seconds;  // seconds
            public int Microseconds; // and microseconds

            public TimeValue(long microSeconds)
            {
                const int microcnv = 1000000;

                Seconds = (int)(microSeconds / microcnv);
                Microseconds = (int)(microSeconds % microcnv);
            }
        }
        private const string WS2_32 = "ws2_32.dll";
        [DllImport(WS2_32, SetLastError = true)]
        internal static extern int select(
                                           [In] int ignoredParameter,
                                           [In, Out] IntPtr[] readfds,
                                           [In, Out] IntPtr[] writefds,
                                           [In, Out] IntPtr[] exceptfds,
                                           [In] ref TimeValue timeout
                                           );

        // Removed (no caller anywhere, signatures wrong on x64): ioctlsocket / WSAIoctl (IOControlCode is a
        // 64-bit enum, the API takes a 32-bit DWORD), WSAStartup + WSAData (Win64 WSADATA has another field
        // order), htonl / ntohl (u_long is 32-bit, were declared ulong), WSALookupServiceNext (HANDLE as Int32),
        // fd_set (unused; select() above uses IntPtr[]).
        [DllImport("ws2_32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        public static extern Int32 WSACleanup();
        [DllImport("Ws2_32.dll")]
        public static extern SocketError WSAGetLastError();
        [DllImport("Ws2_32.dll")]
        public static extern IntPtr socket(AddressFamily af, SocketType type, ProtocolType protocol);
        [DllImport("Ws2_32.dll")]
        public static unsafe extern int send(IntPtr SocketHandle, byte[] buf, int len, int flags);
        [DllImport("Ws2_32.dll")]
        public static extern int recv(IntPtr SocketHandle, byte[] buf, int len, int flags);
        [DllImport("Ws2_32.dll")]
        public static unsafe extern int send([In] IntPtr s, [In] byte* buf, [In] int len, [In] int flags);
        [DllImport("Ws2_32.dll")]
        public static unsafe extern int recv([In] IntPtr s, [Out] byte* buf, [In] int len, [In] int flags);

        [DllImport("ws2_32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        public static extern IntPtr accept(IntPtr socketHandle, ref sockaddr_in socketAddress, ref int addressLength);

        [DllImport("Ws2_32.dll")]
        public static extern int listen(IntPtr s, int backlog);
        [DllImport("Ws2_32.dll", CharSet = CharSet.Ansi)]
        public static extern uint inet_addr(string cp);
        [DllImport("Ws2_32.dll")]
        public static extern ushort htons(ushort hostshort);
        [DllImport("Ws2_32.dll")]
        public static unsafe extern int connect(IntPtr SocketHandle, ref sockaddr_in addr, int addrsize);
        [DllImport("Ws2_32.dll", CharSet = CharSet.Unicode, SetLastError = true)]
        public static extern int closesocket(IntPtr SocketHandle);
        [DllImport("Ws2_32.dll")]
        public static unsafe extern int getpeername(IntPtr SocketHandle, sockaddr_in* addr, int* addrsize);
        [DllImport("Ws2_32.dll")]
        public static unsafe extern int bind(IntPtr SocketHandle, ref sockaddr_in addr, int namelen);
        [DllImport("Ws2_32.dll")]
        public static unsafe extern sbyte* inet_ntoa(int _in);
        [DllImport("Ws2_32.dll")]
        public static unsafe extern ushort ntohs(ushort netshort);
        [DllImport("ws2_32.dll", SetLastError = true)]
        public static extern SocketError setsockopt([In] IntPtr socketHandle, [In] SocketOptionLevel optionLevel, [In] SocketOptionName optionName, [In] ref int optionValue, [In] int optionLength);

        [DllImport("ws2_32.dll", CharSet = CharSet.Unicode, SetLastError = true, CallingConvention = CallingConvention.StdCall)]
        public static extern IntPtr WSASocket(AddressFamily af, SocketType socket_type, ProtocolType protocol,
                                              IntPtr lpProtocolInfo, Int32 group, SocketConstructorFlags dwFlags);

        [DllImport("ws2_32.dll", SetLastError = true)]
        public static extern unsafe int recvfrom([In] IntPtr socketHandle, [In] byte* pinnedBuffer, [In] int len, [In] SocketFlags socketFlags, [Out] byte[] socketAddress, [In, Out] ref int socketAddressSize);

        [DllImport("ws2_32.dll", SetLastError = true)]
        public static extern unsafe int recvfrom([In] IntPtr socketHandle, [In] byte* pinnedBuffer, [In] int len, [In] SocketFlags socketFlags, [Out] sockaddr_in socketAddress, [In, Out] ref int socketAddressSize);

        [DllImport("ws2_32.dll", SetLastError = true)]
        public static extern unsafe int sendto([In] IntPtr socketHandle, [In] byte* pinnedBuffer, [In] int len, [In] SocketFlags socketFlags, [In] byte[] socketAddress, [In] int socketAddressSize);

        [DllImport("ws2_32.dll", CharSet = CharSet.Unicode, SetLastError = true)]
        public static extern int shutdown(IntPtr s, ShutDownFlags how);

        public enum ShutDownFlags : int
        {
            SD_RECEIVE = 0,
            SD_SEND = 1,
            SD_BOTH = 2
        }

        public enum AddressFamily : int
        {
            Unknown = 0,
            InterNetworkv4 = 2,
            Ipx = 4,
            AppleTalk = 17,
            NetBios = 17,
            InterNetworkv6 = 23,
            Irda = 26,
            BlueTooth = 32
        }
        public enum SocketType : int
        {
            Unknown = 0,
            Stream = 1,
            DGram = 2,
            Raw = 3,
            Rdm = 4,
            SeqPacket = 5
        }
        public enum ProtocolType : int
        {
            BlueTooth = 3,
            Tcp = 6,
            Udp = 17,
            ReliableMulticast = 113
        }

        [Flags]
        public enum SocketConstructorFlags
        {
            WSA_FLAG_MULTIPOINT_C_LEAF = 4,
            WSA_FLAG_MULTIPOINT_C_ROOT = 2,
            WSA_FLAG_MULTIPOINT_D_LEAF = 0x10,
            WSA_FLAG_MULTIPOINT_D_ROOT = 8,
            WSA_FLAG_OVERLAPPED = 1
        }


        /// <summary>
        /// Internet socket address structure.
        /// </summary>
        public struct sockaddr_in
        {
            /// <summary>
            /// Protocol family indicator.
            /// </summary>
            public short sin_family;
            /// <summary>
            /// Protocol port.
            /// </summary>
            public ushort sin_port;
            /// <summary>
            /// Actual address value.
            /// </summary>
            public int sin_addr;
            /// <summary>
            /// Address content list.
            /// </summary>
            //   [MarshalAs(UnmanagedType.LPStr, SizeConst=16)]
            //   public string sin_zero;
            public long sin_zero;
        }

        public enum SocketFlags
        {
            Broadcast = 0x400,
            ControlDataTruncated = 0x200,
            DontRoute = 4,
            MaxIOVectorLength = 0x10,
            Multicast = 0x800,
            None = 0,
            OutOfBand = 1,
            Partial = 0x8000,
            Peek = 2,
            Truncated = 0x100
        }
    }
}
